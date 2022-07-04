using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Realmrover
{
    public class GameManager : MonoBehaviour
    {
        public GameState GameState { get; private set; } = Realmrover.GameState.MAIN_MENU;

        [Header("Character Prefab")]
        public GameObject CharacterPrefab;


        [Space(20)]
        [Header("Player Information")]
        public GameObject PlayerPrefab;
        private GameObject _currentPlayer;
        private Character _currentPlayerCharacter;
        public CharacterTemplate PlayerTemplate;

        [Space(20)]
        [Header("Enemy Defaults")]
        [SerializeField]
        private CharacterTemplate _defaultEnemyTemplate;

        [Space(20)]
        [Header("Background Image")]
        [SerializeField]
        private SpriteRenderer _backgroundImage;
        [SerializeField]
        private Sprite _defaultBackgroundImage;

        [Space(20)]
        [Header("Settings Menu")]
        public GameObject SettingsMenuPrefab; 
        private SettingsMenuScript _settingsMenu;

        [Space(20)]
        [Header("Main Menu")]
        public GameObject MainMenuScreenPrefab;
        private MainMenuScript _mainMenuScreen;

        [Space(20)]
        [Header("Main Map")]
        public GameObject MainMapScreenPrefab;
        private GameObject _mainMapObject;
        private MainMapScript _mainMapScreen;

        [Space(20)]
        [Header("Battle Screen")]
        public GameObject BattleScreenPrefab;
        private BattleScreenScript _battleScreen;

        [Space(20)]
        [Header("Levels")]
        [SerializeField]
        private TutorialData _tutorialData;
        private int _currentTutorialStep = 0;
        [SerializeField]
        private List<GameLevel> _gameLevels = new List<GameLevel>();
        private int _currentEnemyInLevelIndex = 0;
        private GameLevel _currentGameLevel;
        private int _currentGameLevelIndex = -1;
        private int _maxLevelUnlocked = 1;

        [Space(20)]
        [Header("Spawn Locations")]
        public Transform EnemySpawnPoint;
        public Transform PlayerSpawnPoint;


        [Space(20)]
        [Header("UI: Canvas")]
        public Canvas DynamicCanvasPrefab;
        private Canvas _dynamicCanvas;
        public Canvas StaticCanvasPrefab;
        private Canvas _staticCanvas;
        public Canvas ScreenTransitionCanvasPrefab;
        private Canvas _screenTransitionCanvas;
        private TransitionCanvasScript _screenTransition;
        [SerializeField]
        private GameObject EndScreenPrefab;

        public GameObject EnemyPrefab;
        private GameObject _currentEnemy;
        private Character _currentEnemyCharacter;
        public CharacterTemplate StartingEnemyTemplate;

        [Space(20)]
        [Header("Floating Combat Text")]
        public GameObject DamageNumberUIPrefab;
        public TextColors TextColorsPreset;

        [Space(20)]
        [Header("Tooltip Prefab")]
        public GameObject TooltipPrefab;
        private TooltipScript _tooltip;

        [Space(20)]
        [Header("Level Name Box Prefab")]
        public GameObject LevelNameBoxPrefab;
        private LevelNameBoxScript _levelNameBox;

        [Space(20)]
        [Header("Sound")]
        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private AudioClip _clickSound;
        [SerializeField]
        private AudioSource _musicSource;
        [SerializeField]
        private AudioClip _battleTrack;

        private bool _playerTurn = true;
        private float _maxAbilityRechargeTime = 1f;
        private float _abilityRechargeTime = 0.5f;
        private const float TurnDelay = 1f;
        private float _turnDelayTime = 1f;
        //private GameState GameState = Realmrover.GameState.MAIN_MENU;
        private bool _gameStateDirty = true;
        private Queue<FloatingTextQueueMessage> _damageNumbersQueue = new Queue<FloatingTextQueueMessage>();
        private float _floatingTextDelay = 0.4f;
        private float _floatingTextTime = 0.4f;
        private GameState _previousGameState;

        private bool _battleControlButtonPressed = false;
        private bool _EndTurnButtonPressed = false;
        private bool _nextBattleButtonPressed = false;
        private bool _leaveBattleButtonPressed = false;
        private bool _levelSelected = false;
        private bool _newGameStarted = false;
        private float _soundVolume = 1f;
        private float _musicVolume = 1f;
        private bool _tutorialDone = false;
        private Skill _lastAbilityUsed;
        private bool _tutorialStarted = false;
        private int _lastAbilityShown = 0;
        private bool _messageNextButtonPressed = false;
        private void Awake()
        {
            Initialize();
        }
        private void Update()
        {
            HandleAbilityRecharge();
            HandleGame();

            if(Input.GetKeyDown(KeyCode.T))
            {
                ShowTooltip("This is a tutorial. Click Ok to continue.");
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                ShakeCamera(0.5f, 2f);
            }
        }

        public void MessageNextButtonPressed()
        {
            if(GameState == GameState.MAIN_MAP)
            {
                return;
            }
            GameState = _previousGameState;
            _tooltip.Clear();
            HideTooltip();
            _messageNextButtonPressed = true;

        }
        private void CheckIfTutorialDone()
        {
            int check = 0;
            if(PlayerPrefs.HasKey("TutorialDone"))
            {
                check = PlayerPrefs.GetInt("TutorialDone");
            }

            if(check == 1)
            {
                _mainMapScreen.UpdateLayout(2);
                _maxLevelUnlocked++;
            }
        }

        //To be called first in Awake()
        private void Initialize()
        {

            InitializeCanvases();
            InitializeSettingsMenu();
            InitializeTooltip();
            InitializeLevelNameBox();
            InitializeMainMenuScreen();
            InitializeSFX();
            InitializeMainMapScreen();
            InitializeBattleScreen();
            InitializePlayer();
            InitializeEnemy();


            CheckIfTutorialDone();


            //ToggleEnemyCharacter(false);
            //TogglePlayerCharacter(false);
            //EnterGame();
        }

        private void Start()
        {
            EnterGame();
        }

        private void InitializeSFX()
        {
            _soundVolume = PlayerPrefs.GetFloat("SoundVolume");
            _musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            _audioSource.volume = _soundVolume;
            _musicSource.volume = _musicVolume;
        }

        public void UpdateAudioSourceVolume()
        {
            _audioSource.volume = PlayerPrefs.GetFloat("SoundVolume");
            _musicSource.volume = PlayerPrefs.GetFloat("MusicVolume");
        }
        //To be called every frame in Update()
        private void HandleGame()
        {
            HandleBattle();
        }
        public void PlayClickSound()
        {
            _audioSource.PlayOneShot(_clickSound);
        }

        public void PlayBattleMusic(bool value)
        {
            if(value == true)
            {
                _musicSource.mute = false;
            } else
            {
                _musicSource.mute = true;
            }

            
        }
        private void HandleBattle()
        {

            if(GameState == GameState.SHOWING_MESSAGE)
            {
                return;
            }

            HandleFloatingText();

            if (GameState == GameState.BATTLE_TRANSITION)
            {
                return;
            }

            if(GameState == GameState.SCREEN_TRANSITION)
            {
                return;
            }

            if(GameState == GameState.MAIN_MENU)
            {
                if(_newGameStarted == true)
                {
                    _battleScreen.UpdatePlayerSkillBar(_currentPlayerCharacter);
                    ToggleAll(false);
                    ChangeGameState(GameState.ENTERING_MAIN_MAP);
                    ToggleMainMapScreen(true);
                    _newGameStarted = false;
 
                }
            }

            if(GameState == GameState.ENTERING_MAIN_MAP)
            {
                ChangeGameState(GameState.MAIN_MAP);
                
            }

            if(GameState == GameState.INTRODUCING_LEVEL)
            {
                if (_currentGameLevelIndex == -1)
                {
                    HideAllAbilityIcons();

                }

                _battleControlButtonPressed = false;
                ToggleAll(false);
                ToggleBattleScreen(true);
                _battleScreen.ToggleEnemyResources(false);
                _currentEnemyCharacter.FadeIn(TurnDelay);
                _battleScreen.UpdateBackground(_gameLevels[_currentGameLevelIndex].Background);
                ToggleLevelNameBox(true);
                _levelNameBox.SetName(_gameLevels[_currentGameLevelIndex].Name);
                _levelNameBox.FadeOut(0f);
                _levelNameBox.FadeIn(TurnDelay);
                ChangeGameState(GameState.BATTLE_ENTERED);

            }

            if(GameState == GameState.BATTLE_ENTERED)
            {
                
                ToggleBattleScreen(true);
                _battleScreen.SetButtonText(BattleControlButtonTextType.EMPTY);
                TogglePlayerCharacter(true);
                ToggleEnemyCharacter(true);
                _battleScreen.ToggleEnemyResources(true);
                UpdateResourcesUI();
                _levelNameBox.FadeOut(TurnDelay*2);
                ChangeGameState(GameState.BATTLE_START, TurnDelay);
            }

            if (GameState == GameState.BATTLE_START)
            {
                if (_currentGameLevelIndex == -1)
                {
                    _currentGameLevelIndex++;
                }

                if (_currentGameLevelIndex == 0)
                {
                    StartTutorial();
                }
                _battleScreen.SetButtonText(BattleControlButtonTextType.END_TURN);
                ChangeGameState(GameState.BATTLE_PLAYER_TURN, TurnDelay);
                
            }

            if (WaitForTutorialCondition() == true && _currentGameLevelIndex == 0)
            {
                Debug.Log("Tutorial condition true");
                ContinueTutorial();
            }

            if (GameState == GameState.BATTLE_PLAYER_TURN)
            {

                _battleScreen.SetButtonText(BattleControlButtonTextType.END_TURN);
                HandlePlayerInput();
                if (_battleControlButtonPressed == true)
                {
                    EndTurn(_currentPlayerCharacter);
                    _battleControlButtonPressed = false;
                    ChangeGameState(GameState.BATTLE_ENEMY_TURN, TurnDelay);
                }
            }

            if (GameState == GameState.BATTLE_ENEMY_TURN)
            {
                _battleScreen.SetButtonText(BattleControlButtonTextType.ENEMY_TURN);
                HandleEnemyAI();
                EndTurn(_currentEnemyCharacter);
                ChangeGameState(GameState.BATTLE_PLAYER_TURN, TurnDelay);
            }

            if(GameState == GameState.BATTLE_PLAYER_WIN)
            {
                _currentEnemyInLevelIndex++;
                _battleControlButtonPressed = false;
                if (IsEndOfLevel() == true)
                {
                    
                    _battleScreen.SetButtonText(BattleControlButtonTextType.EMPTY);
                    ChangeGameState(GameState.END_OF_LEVEL, TurnDelay);
                } else
                {
                    
                    ChangeGameState(GameState.SPAWNING_NEXT_ENEMY);
                }
    
            }

            if(GameState == GameState.SPAWNING_NEXT_ENEMY)
            {
                _battleScreen.SetButtonText(BattleControlButtonTextType.NEXT_BATTLE);
                if (_battleControlButtonPressed == true)
                {
                    _battleControlButtonPressed = false;
                    SpawnNextEnemy();                    
                    ChangeGameState(GameState.BATTLE_START, TurnDelay);
                    
                }
            }

            if (GameState == GameState.END_OF_LEVEL)
            {
                _battleScreen.ToggleEnemyResources(false);
                _battleScreen.SetButtonText(BattleControlButtonTextType.BACK_TO_MAIN_MAP);
                if(_currentGameLevelIndex == 2)
                {
                    ChangeGameState(GameState.END_SCREEN, TurnDelay);
                }
                if(_battleControlButtonPressed == true)
                {
                    _battleControlButtonPressed = false;
                    ToggleAll(false);
                    ToggleMainMapScreen(true);
                    _currentPlayerCharacter.EndBattle();
                    _currentEnemyCharacter.EndBattle();
                    ChangeGameState(GameState.MAIN_MAP, TurnDelay);

                    if (_currentGameLevelIndex == 0)
                    {
                        _maxLevelUnlocked = 2;
                    } else
                    {
                        _maxLevelUnlocked++;
                    }

                    _mainMapScreen.UpdateLayout(_maxLevelUnlocked);
                    //_currentTutorialStep = 0;

                    RestartTutorial();
                }

            }

            if(GameState == GameState.BATTLE_PLAYER_DEFEAT)
            {
                
                _battleScreen.SetButtonText(BattleControlButtonTextType.BACK_TO_MAIN_MAP);
                if(_battleControlButtonPressed == true)
                {
                    
                    _battleControlButtonPressed = false;
                    _currentPlayerCharacter.EndBattle();
                    _currentEnemyCharacter.EndBattle();
                    ToggleAll(false);
                    ToggleMainMapScreen(true);
                    ChangeGameState(GameState.MAIN_MAP, TurnDelay);
                    _currentPlayerCharacter.SetCharacterTemplate(PlayerTemplate);
                }
            }

            if(GameState == GameState.END_SCREEN)
            {
                ToggleAll(false);
                Instantiate(EndScreenPrefab, _dynamicCanvas.transform);
                ChangeGameState(GameState.SCREEN_TRANSITION);
                PlayBattleMusic(false);
            }


        }

        private void HandleScreenTransitions()
        {
            if(GameState == GameState.SCREEN_TRANSITION)
            {

            }
        }
        private void HandleFloatingText()
        {
            if (_floatingTextTime > 0)
            {
                _floatingTextTime -= Time.deltaTime;
            }

            if(_damageNumbersQueue.Count > 0 && FloatingTextReady() )
            {
                ShowNextCombatText();
                _floatingTextTime = _floatingTextDelay;
            }
        }

        private bool FloatingTextReady()
        {
            return _floatingTextTime <= 0f;
        }
        private void HandleEnemyAI()
        {
            _currentEnemyCharacter.MakeNextMove(_currentPlayerCharacter);

        }
        private bool IsEndOfLevel()
        {
            return _currentEnemyInLevelIndex >= _gameLevels[_currentGameLevelIndex].Enemies.Count;
        }
        private void InitializeMainMenuScreen()
        {
            if (_mainMenuScreen != null)
            {
                return;
            }

            var spawn = Instantiate(MainMenuScreenPrefab, _staticCanvas.transform);
            _mainMenuScreen = spawn.GetComponent<MainMenuScript>();
            _mainMenuScreen.Initialize(this);
        }
        private void InitializeMainMapScreen()
        {
            if (_mainMapScreen != null)
            {
                return;
            }

            var spawn = Instantiate(MainMapScreenPrefab, _staticCanvas.transform);
            _mainMapObject = spawn;
            _mainMapScreen = spawn.GetComponent<MainMapScript>();
            _mainMapScreen.Initialize(this);
        }
        private void InitializeBattleScreen()
        {
            if (_battleScreen != null)
            {
                return;
            }

            var spawn = Instantiate(BattleScreenPrefab, _staticCanvas.transform);
            _battleScreen = spawn.GetComponent<BattleScreenScript>();
            _battleScreen.Initialize(this);
        }
        private void InitializeCanvases()
        {
            _dynamicCanvas = Instantiate(DynamicCanvasPrefab);
            _dynamicCanvas.worldCamera = Camera.main;
            _staticCanvas = Instantiate(StaticCanvasPrefab);
            _staticCanvas.worldCamera = Camera.main;
            _screenTransitionCanvas = Instantiate(ScreenTransitionCanvasPrefab);
            _screenTransitionCanvas.worldCamera = Camera.main;
            _screenTransition = _screenTransitionCanvas.GetComponent<TransitionCanvasScript>();
        }
        private void InitializeSettingsMenu()
        {
            var spawn = Instantiate(SettingsMenuPrefab, _dynamicCanvas.transform);
            _settingsMenu = spawn.GetComponent<SettingsMenuScript>();
            _settingsMenu.Initialize(this);
            _settingsMenu.gameObject.SetActive(false);
        }
        private void InitializeTooltip()
        {
            if (_tooltip != null)
            {
                return;
            }

            var spawn = Instantiate(TooltipPrefab, _dynamicCanvas.transform);
            _tooltip = spawn.GetComponent<TooltipScript>();
            _tooltip.Initialize(this);
            HideTooltip();

        }
        private void InitializeLevelNameBox()
        {
            if(_levelNameBox != null)
            {
                return;
            }

            var spawn = Instantiate(LevelNameBoxPrefab, _dynamicCanvas.transform);
            _levelNameBox = spawn.GetComponent<LevelNameBoxScript>();
            _levelNameBox.Initialize(this);
            ToggleLevelNameBox(false);
        }
        private void ToggleLevelNameBox(bool value)
        {
            _levelNameBox.gameObject.SetActive(value);
        }
        private void InitializePlayer(int level = 1)
        {
            InitializeCharacter(true, level);
        }

        private void InitializeCharacter(bool IsPlayer = false, int playerLevel = 1)
        {
            if (IsPlayer == true)
            {
                if (_currentPlayer != null)
                {
                    return;
                }

                _currentPlayer = Instantiate(CharacterPrefab, PlayerSpawnPoint);
                _currentPlayerCharacter = _currentPlayer.GetComponent<Character>();
                _currentPlayerCharacter.Initialize(this);
                _currentPlayerCharacter.SetCharacterTemplate(PlayerTemplate);

                if (_currentPlayerCharacter == null)
                {
                }
            } else
            {
                if (_currentEnemy != null)
                {
                    return;
                }

                _currentEnemy = Instantiate(CharacterPrefab, EnemySpawnPoint);
                _currentEnemyCharacter = _currentEnemy.GetComponent<Character>();
                _currentEnemyCharacter.Initialize(this);

            }
        }
        private void InitializeEnemy()
        {
            InitializeCharacter();
        }
        private void TogglePlayerCharacter(bool value)
        {
            ToggleObject(_currentPlayer, value);
        }
        public void ShowSettingsMenu()
        {
            _settingsMenu.gameObject.SetActive(true);
        }
        private void ToggleEnemyCharacter(bool value)
        {
            ToggleObject(_currentEnemy, value);
        }

        private void ToggleObject(GameObject obj, bool value)
        {
            if (obj == null)
            {
                return;
            }
            if (obj.gameObject.activeSelf == value)
            {
                return;
            }

            obj.SetActive(value);
        }

        public void QueueCombatText(int value, FloatingTextType textType, bool atEnemy)
        {
            _damageNumbersQueue.Enqueue(new FloatingTextQueueMessage(value, textType, atEnemy));
        }

        private void ShowNextCombatText()
        {
            var nextDamageNumber = _damageNumbersQueue.Dequeue();

            var spawn = Instantiate(DamageNumberUIPrefab, _dynamicCanvas.transform);
            var damageNumber = spawn.GetComponent<DamageNumber>();
            var location = nextDamageNumber.AtEnemy ? EnemySpawnPoint : PlayerSpawnPoint;
            damageNumber.Initialize(nextDamageNumber.Value, nextDamageNumber.TextType, location.position, this);
        }

        public void EndTurn(Character endedByCharacter)
        {
            _currentPlayerCharacter.EndTurn();
            _currentEnemyCharacter.EndTurn();
            UpdateResourcesUI();
        }

        public void UpdateResourcesUI()
        {
            _battleScreen.UpdatePlayerResources(_currentPlayerCharacter);
            _battleScreen.UpdateEnemyResources(_currentEnemyCharacter);

        }
        public void BattleControlButtonPressed()
        {

            _battleControlButtonPressed = true;

        }
        public void ActivateAbility(int index)
        {
            if (GameState != GameState.BATTLE_PLAYER_TURN)
            {
                return;
            }

            if (IsRecharging())
            {
                return;
            }
            if (_currentPlayerCharacter.ActivateSkill(index, _currentEnemyCharacter) == true)
            {
                TriggerAbilityRecharge();
                _lastAbilityUsed = _currentPlayerCharacter.Skills()[index];
            }

        }
        private void HandlePlayerInput()
        {
            if(_currentPlayerCharacter.IsAlive() == false)
            {
                ChangeGameState(GameState.BATTLE_PLAYER_DEFEAT);
                return;
            }

            if (GameState != GameState.BATTLE_PLAYER_TURN )
            {
                return;
            }

            if (IsRecharging())
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ActivateAbility(0);
            }


            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ActivateAbility(1);
            }


            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ActivateAbility(2);
            }


            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ActivateAbility(3);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                ActivateAbility(4);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                ActivateAbility(5);
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                ActivateAbility(6);
            }
        }

        private void TriggerAbilityRecharge(float rechargeTime = 1f)
        {
            _abilityRechargeTime = Mathf.Clamp(rechargeTime, 0, 10000f);

        }

        private void HandleAbilityRecharge()
        {
            if (_abilityRechargeTime > 0)
            {
                _abilityRechargeTime -= Time.deltaTime;
            }
        }

        private bool IsRecharging()
        {
            return _abilityRechargeTime >= 0.1f;
        }

        private void ChangeGameState(GameState newState, float delay = 0f, bool transition = false)
        {
            
            if(delay == 0f)
            {
                GameState = newState;
                return;
            }

            StartCoroutine(SwitchStateDelayCO(newState, delay, transition));

        }

        private IEnumerator SwitchStateDelayCO(GameState newState, float delay, bool transition)
        {
            GameState = GameState.BATTLE_TRANSITION;
            if (transition == true)
            {
                StartCoroutine(ScreenTransitionCO(delay/2f, true));
                yield return new WaitForSeconds(delay / 2f);
                StartCoroutine(ScreenTransitionCO(delay / 2f, false));
            } else
            {
                yield return new WaitForSeconds(delay);
            }
            yield return new WaitForSeconds(delay/2f);
            GameState = newState;
        }

        private IEnumerator ScreenTransitionCO(float duration, bool fadeIn)
        {
            if(duration == 0f)
            {
                yield break;
            }
            float originalDuration = duration;
            Image image = _screenTransition.ScreenPanel();
            while (duration > 0)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, fadeIn? (1f - (duration / originalDuration)) :(duration / originalDuration));
                duration -= Time.deltaTime;
                yield return null;
            }
        }

        public void SwitchPlayerClass(CharacterTemplate template)
        {
            PlayerTemplate = template;
        }

        public Character CurrentPlayerCharacter()
        {
            return _currentPlayerCharacter;
        }

        public Character CurrentEnemyCharacter()
        {
            return _currentEnemyCharacter;
        }

        public void EnterGame()
        {
            ToggleAll(false);
            ToggleMainMenuScreen(true);
        }

        public void NewGameButtonPressed()
        {
            _newGameStarted = true;
            /*ToggleAll(false);
            _battleScreen.UpdatePlayerSkillBar(_currentPlayerCharacter);
            ChangeGameState(GameState.MAIN_MAP, TurnDelay, true);
            ToggleMainMapScreen(true);*/
        }

        public void LevelSelected(int index)
        {
    

            if(_gameLevels == null || _gameLevels.Count == 0)
            {
                return;
            }

            if(index >= _gameLevels.Count || index <0)
            {
                return;
            }

            
            _currentGameLevelIndex = index;
  
            _levelSelected = true;
            _currentEnemyInLevelIndex = 0;
            var enemyTemplateToSpawn = _gameLevels[index].Enemies[_currentEnemyInLevelIndex].Template;

            _currentEnemyCharacter.SetCharacterTemplate(enemyTemplateToSpawn);
            ChangeGameState(GameState.INTRODUCING_LEVEL, TurnDelay, true);
            PlayBattleMusic(true);
        }

        private void UpdateCurrentBattleBackground()
        {

        }

        public bool SpawnNextEnemy()
        {
            var enemyList = _gameLevels[_currentGameLevelIndex].Enemies;
            if (enemyList == null || _currentEnemyInLevelIndex >= enemyList.Count)
            {
                return false;
            }
            
            StartCoroutine(SpawnEnemyCO(enemyList[_currentEnemyInLevelIndex].Template));
            
            return true;
        }

        

        private IEnumerator SpawnEnemyCO(CharacterTemplate characterTemplate)
        {
            CurrentEnemyCharacter().FadeOut(TurnDelay);
            _battleScreen.ToggleEnemyResources(false);
            yield return new WaitForSeconds(TurnDelay);
            //_currentEnemyInLevelIndex++;
            _currentEnemyCharacter.EndBattle();
            _currentEnemyCharacter.SetCharacterTemplate(characterTemplate, _gameLevels[_currentGameLevelIndex].Enemies[_currentEnemyInLevelIndex].Level);
            CurrentEnemyCharacter().FadeIn(TurnDelay);
            yield return new WaitForSeconds(TurnDelay);
            _battleScreen.ToggleEnemyResources(true);
            UpdateResourcesUI();
            //ChangeGameState(GameState.BATTLE_START);
            
        }

        public void ReportCharacterDeath(Character character)
        {
            if(character == null)
            {
                return;
            }

            if(character == _currentPlayerCharacter)
            {
                ChangeGameState(GameState.BATTLE_PLAYER_DEFEAT);
                
            }

            if(character == _currentEnemyCharacter)
            {
                ChangeGameState(GameState.BATTLE_PLAYER_WIN, TurnDelay);
            }

        }

        private void ToggleMainMenuScreen(bool value)
        {
            ToggleObject(_mainMenuScreen.gameObject, value);
        }
        private void ToggleMainMapScreen(bool value)
        {
            ToggleObject(_mainMapScreen.gameObject, value);
            if(value == true)
            {
                PlayBattleMusic(false);
            }
        }
        
        private void ToggleBattleScreen(bool value)
        {
            ToggleObject(_battleScreen.gameObject, value);

            if (value == true)
            {
                _battleScreen.UpdatePlayerResources(_currentPlayerCharacter);
                _battleScreen.UpdateEnemyResources(_currentEnemyCharacter);
            } else
            {
                _damageNumbersQueue.Clear();
            }

            PlayBattleMusic(value);
            ToggleEnemyCharacter(value);
            TogglePlayerCharacter(value);
        }
        private void ToggleAll(bool value)
        {
            ToggleMainMapScreen(value);
            ToggleMainMenuScreen(value);
            ToggleBattleScreen(value);
            TogglePlayerCharacter(value);
            ToggleEnemyCharacter(value);
        }

        public void ShowTooltip(Character character)
        {
            if (character == null)
            {
                return;
            }
            _tooltip.gameObject.SetActive(true);
            _tooltip.SetTooltip(character);
        }
        public void ShowTooltip(Skill skill)
        {
            if(skill == null || GameState == GameState.SHOWING_MESSAGE)
            {
                return;
            }
            _tooltip.gameObject.SetActive(true);
            _tooltip.SetTooltip(skill);
        }

        public void ShowTooltip(string message)
        {
            if(message == null)
            {
                return;
            }
            _previousGameState = GameState;
            ChangeGameState(GameState.SHOWING_MESSAGE);
            _tooltip.gameObject.SetActive(true);
            _tooltip.SetTooltip(message);
        }

        public void HideTooltip()
        {
            if(GameState == GameState.SHOWING_MESSAGE)
            {
                return;
            }
            _tooltip.gameObject.SetActive(false);
        }

        public void PlaySound(Skill skill)
        {
            if(skill == null || skill.SoundEffect == null)
            {
                return;
            }
            _audioSource.PlayOneShot(skill.SoundEffect);
        }

        public void ShakeCamera(float amount, float duration = 1f)
        {
            if(duration <= 0)
            {
                return;
            }
            StartCoroutine(ShakeCameraCO(amount, duration));
        }

        private IEnumerator ShakeCameraCO(float amount, float duration, float delay = 0.5f)
        {
            Camera cam = Camera.main;
            float originalDuration = duration;
            //Vector3 originalPos = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
            yield return new WaitForSeconds(delay);
            amount /= 3f;
            while (duration > 0)
            {
                amount /= 1.3f;
                Vector3 pos = new Vector3(Random.Range(-amount, amount), Random.Range(-amount, amount), cam.transform.position.z);
                //pos = Vector3.Lerp(pos, originalPos, (1-duration) / originalDuration);
                cam.transform.position = pos;
                duration -= Time.deltaTime;
                
                yield return null;
            }
            cam.transform.localPosition = Vector3.zero;
        }

        public void StartTutorial()
        {
            if(_currentTutorialStep > 0 )
            {
                return;
            }
            HideAllAbilityIcons();
            ShowTooltip(_tutorialData.TutorialSteps[_currentTutorialStep].Message);
            //_currentTutorialStep++;

        }

        private void HideAllAbilityIcons()
        {
            for (int i = 1; i < 7; i++)
            {
                _battleScreen.ToggleAbility(i, false);
            }
        }

        public void RestartTutorial()
        {
            _currentTutorialStep = 0;
        }

        public bool WaitForTutorialCondition()
        {
            if(GameState == GameState.MAIN_MAP || GameState == GameState.ENTERING_MAIN_MAP)
            {
                return false;
            }

            if(_currentTutorialStep >= _tutorialData.TutorialSteps.Count)
            {
                PlayerPrefs.SetInt("TutorialDone", 1);
                Debug.Log("tutorial finished");
                return false;
            }

            var currentTutorialStep = _tutorialData.TutorialSteps[_currentTutorialStep];

            if(currentTutorialStep.WaitForEndOfTurn == false)
            {
                if(currentTutorialStep.WaitForSkill == null)
                {
                    if(_messageNextButtonPressed)
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                } else if(_lastAbilityUsed == currentTutorialStep.WaitForSkill)
                {
                    return true;
                } else
                {
                    return false;
                }
 
            } else 
            {
                if(_battleControlButtonPressed == true)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
            /*if(currentTutorialStep.WaitForEndOfTurn == true)
            {
                if(_battleControlButtonPressed == true)
                {
                    
                    return true;
                } else
                {
                    return false;
                }
            } else if (currentTutorialStep.WaitForSkill == null)
            {

            }
                if(currentTutorialStep.WaitForSkill == null)
                {
                    if(_messageNextButtonPressed == true)
                    {
                        _messageNextButtonPressed = false;
                        return true;
                    }

                } else if(_lastAbilityUsed == currentTutorialStep.WaitForSkill)
                {
                    return true;
                } else
                {
                    return false;
                }*/
                //return false;
            

        }

        public void ContinueTutorial()
        {
            _lastAbilityUsed = null;
            _currentTutorialStep++;
            
            if (_currentTutorialStep >= _tutorialData.TutorialSteps.Count)
            {
                _currentEnemyCharacter.TakeDamage(100000, FloatingTextType.DIRECT_DAMAGE, _currentPlayerCharacter, _currentPlayerCharacter.Skills()[0], true, true, true);
                return;
            }

            ShowTooltip(_tutorialData.TutorialSteps[_currentTutorialStep].Message);
            if (_tutorialData.TutorialSteps[_currentTutorialStep].WaitForSkill != null)
            {
                ShowNextAbilityInTutorial();
            }
        }
        public void ShowNextAbilityInTutorial()
        {
            if(_lastAbilityShown >= 7)
            {
                return;
            }

            _battleScreen.ToggleAbility(_lastAbilityShown, true);
            _lastAbilityShown++;
        }

        public GameLevel GameLevelByIndex(int index)
        {
            if (index < 0 || index >= _gameLevels.Count)
            {
                return null;
            }

            else return _gameLevels[index];
        }
    }


    public enum FloatingTextType
    {
        DIRECT_DAMAGE,
        DIRECT_HEAL,
        HEALTH_REGEN,
        ENERGY_REGEN,
        HEAL_PER_TURN,
        DAMAGE_PER_TURN,
        COMPLETE_ABSORB,
        REFLECT,
        SACRIFICE
    }

    public enum GameState
    {
        MAIN_MENU,
        MAIN_MENU_SETTINGS,
        MAIN_MAP,
        BATTLE_ENTERED,
        BATTLE_START,
        BATTLE_PLAYER_TURN,
        BATTLE_ENEMY_TURN,
        BATTLE_PLAYER_DEFEAT,
        BATTLE_PLAYER_WIN,
        END_OF_LEVEL,
        END_SCREEN,
        BATTLE_TRANSITION,
        SCREEN_TRANSITION,
        SCREEN_TRANSITIONING,
        SHOWING_MESSAGE,
        INTRODUCING_LEVEL,
        ENTERING_MAIN_MAP,
        SPAWNING_NEXT_ENEMY,
    }

    public class FloatingTextQueueMessage
    {
        public int Value;
        public FloatingTextType TextType;
        public bool AtEnemy;

        public FloatingTextQueueMessage(int value, FloatingTextType textType, bool atEnemy)
        {
            Value = value;
            TextType = textType;
            AtEnemy = atEnemy;
        }
    }
}