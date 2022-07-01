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
        private List<GameLevel> _gameLevels = new List<GameLevel>();
        private int _currentEnemyInLevelIndex = 0;
        private GameLevel _currentGameLevel;
        private int _currentGameLevelIndex = 0;
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

        private float _soundVolume = 1f;
        private float _musicVolume = 1f;

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
            ChangeGameState(_previousGameState);
            _tooltip.Clear();
            HideTooltip();

        }

        //To be called first in Awake()
        private void Initialize()
        {

            InitializeCanvases();
            InitializeSettingsMenu();
            InitializeTooltip();
            InitializeMainMenuScreen();
            InitializeSFX();
            InitializeMainMapScreen();
            InitializeBattleScreen();
            InitializePlayer();
            InitializeEnemy();
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
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("SoundVolume", _soundVolume);
            PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
        }
        //To be called every frame in Update()
        private void HandleGame()
        {
            if(GameState == GameState.BATTLE_PLAYER_TURN)
            {

            }
            HandleBattle();
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

            if (GameState == GameState.BATTLE_START)
            {
                // First turn is decided here
                _battleScreen.SetButtonText(BattleControlButtonTextType.END_TURN);
                ChangeGameState(GameState.BATTLE_PLAYER_TURN, TurnDelay);
            }

            if(GameState == GameState.BATTLE_PLAYER_TURN)
            {
                _battleScreen.SetButtonText(BattleControlButtonTextType.END_TURN);
                HandlePlayerInput();
            }

            

            if(GameState == GameState.BATTLE_ENEMY_TURN)
            {
                _battleScreen.SetButtonText(BattleControlButtonTextType.ENEMY_TURN);
                HandleEnemyAI();
            }

            if(GameState == GameState.BATTLE_PLAYER_WIN)
            {
                if(IsEndOfLevel())
                {
                    ChangeGameState(GameState.END_OF_LEVEL);
                    _battleScreen.SetButtonText(BattleControlButtonTextType.BACK_TO_MAIN_MAP);

                } else
                {
                    _battleScreen.SetButtonText(BattleControlButtonTextType.NEXT_BATTLE);
                    if (_battleControlButtonPressed == true)
                    {
                        _battleControlButtonPressed = false;
                        if (SpawnNextEnemy())
                        {
                            ChangeGameState(GameState.BATTLE_START, TurnDelay);
                            _battleScreen.SetButtonText(BattleControlButtonTextType.EMPTY);
                        }
                        else
                        {
                            ChangeGameState(GameState.END_OF_LEVEL, TurnDelay);
                            _battleScreen.SetButtonText(BattleControlButtonTextType.EMPTY);
                        }

                        
                    }

                }
            }

            if (GameState == GameState.END_OF_LEVEL)
            {
                _battleScreen.SetButtonText(BattleControlButtonTextType.BACK_TO_MAIN_MAP);
                if(_battleControlButtonPressed == true)
                {
                    _battleControlButtonPressed = false;
                    ToggleAll(false);
                    ToggleMainMapScreen(true);
                    _currentPlayerCharacter.EndBattle();
                    _currentEnemyCharacter.EndBattle();
                    ChangeGameState(GameState.MAIN_MAP, TurnDelay);
                    _maxLevelUnlocked++;
                    _mainMapScreen.UpdateLayout(_maxLevelUnlocked);

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

                }
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
            EndTurn(_currentEnemyCharacter);
        }
        private bool IsEndOfLevel()
        {
            return _gameLevels[_currentGameLevelIndex].Enemies.Count == (_currentEnemyInLevelIndex-1) ;
        }
        private void InitializeMainMenuScreen()
        {
            if (_mainMenuScreen != null)
            {
                Debug.LogWarning("Main Menu Screen is already initialized.");
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
                Debug.LogWarning("Main Map Screen is already initialized.");
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
                Debug.LogWarning("Battle Screen is already initialized.");
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
                Debug.LogWarning("Tooltip is already initialized.");
                return;
            }

            var spawn = Instantiate(TooltipPrefab, _dynamicCanvas.transform);
            _tooltip = spawn.GetComponent<TooltipScript>();
            _tooltip.Initialize(this);
            HideTooltip();

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
                    Debug.LogWarning("Player object is already initialized!");
                    return;
                }

                _currentPlayer = Instantiate(CharacterPrefab, PlayerSpawnPoint);
                _currentPlayerCharacter = _currentPlayer.GetComponent<Character>();
                _currentPlayerCharacter.Initialize(this);
                _currentPlayerCharacter.SetCharacterTemplate(PlayerTemplate);

                if (_currentPlayerCharacter == null)
                {
                    Debug.LogWarning("Initialize Character Error: Player Character component missing from CharacterPrefab!");
                }
            } else
            {
                if (_currentEnemy != null)
                {
                    Debug.LogWarning("Enemy object is already initialized!");
                    return;
                }

                _currentEnemy = Instantiate(CharacterPrefab, EnemySpawnPoint);
                _currentEnemyCharacter = _currentEnemy.GetComponent<Character>();
                _currentEnemyCharacter.Initialize(this);
                if (_currentEnemyCharacter == null)
                {
                    Debug.LogWarning("Initialize Character Error: Enemy Character component missing from CharacterPrefab!");
                }

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
                Debug.LogWarning("Toggling Object: Object is null!");
                return;
            }
            if (obj.gameObject.activeSelf == value)
            {
                Debug.LogWarning("Toggling Object: Object is already " + (value? "on!": "off!"));
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
            if (GameState == GameState.BATTLE_PLAYER_TURN || GameState == GameState.BATTLE_ENEMY_TURN)
            {

                if (endedByCharacter == _currentPlayerCharacter)
                {
                    _currentPlayerCharacter.EndTurn();
                    ChangeGameState(GameState.BATTLE_ENEMY_TURN, TurnDelay);
                }
                else if (endedByCharacter == _currentEnemyCharacter)
                {
                    _currentEnemyCharacter.EndTurn();
                    ChangeGameState(GameState.BATTLE_PLAYER_TURN, TurnDelay);
                }

                UpdateResourcesUI();
            }
        }

        public void UpdateResourcesUI()
        {
            _battleScreen.UpdatePlayerResources(_currentPlayerCharacter);
            _battleScreen.UpdateEnemyResources(_currentEnemyCharacter);

        }
        public void BattleControlButtonPressed()
        {
            if (GameState == GameState.BATTLE_TRANSITION || GameState == GameState.SCREEN_TRANSITION)
            {
                _battleControlButtonPressed = false;
                return;
            }
            _battleControlButtonPressed = true;

            /*if (GameState == GameState.BATTLE_PLAYER_TURN)
            {
                EndTurn(_currentPlayerCharacter);
            } else if(GameState == GameState.END_OF_LEVEL)
            {
                ToggleAll(false);
                ToggleMainMapScreen(true);
                _currentPlayerCharacter.EndBattle();
                _currentEnemyCharacter.EndBattle();
                ChangeGameState(GameState.MAIN_MAP, TurnDelay);
            } else if(GameState == GameState.BATTLE_PLAYER_DEFEAT)
            {
                _currentPlayerCharacter.EndBattle();
                _currentEnemyCharacter.EndBattle();
                ToggleAll(false);
                ToggleMainMapScreen(true);
                ChangeGameState(GameState.MAIN_MAP, TurnDelay);

            } else if(GameState == GameState.BATTLE_PLAYER_WIN)
            {
                if (SpawnNextEnemy())
                {
                    ChangeGameState(GameState.BATTLE_START, TurnDelay);

                }
                else
                {
                    ChangeGameState(GameState.END_OF_LEVEL, TurnDelay);
                }
            }*/

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
            }

        }
        private void HandlePlayerInput()
        {
            if (GameState != GameState.BATTLE_PLAYER_TURN )
            {
                return;
            }

            if (IsRecharging())
            {
                return;
            }

            if(_battleControlButtonPressed == true)
            {
                EndTurn(_currentPlayerCharacter);
                _battleControlButtonPressed = false;
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

        private void ChangeGameState(GameState newState, float delay = 0f, bool transition = true)
        {
            if(delay == 0f)
            {
                GameState = newState;
                return;
            }
            if (transition == true)
            {
                StartCoroutine(SwitchStateDelayCO(newState, delay));
            } else
            {
                GameState = newState;
            }

        }

        private IEnumerator SwitchStateDelayCO(GameState newState, float delay)
        {
            GameState = GameState.BATTLE_TRANSITION;
            //StartCoroutine(ScreenTransitionCO(delay));
            yield return new WaitForSeconds(delay);
            GameState = newState;
            Debug.Log("Game State Changed to :   " + GameState.ToString());
        }

        private IEnumerator ScreenTransitionCO(float duration)
        {
            if(duration == 0f)
            {
                yield break;
            }
            float originalDuration = duration;
            Image image = _screenTransition.ScreenPanel();
            while (duration > 0)
            {
                image.color = new Color(0, 0, 0, Mathf.Sin((1-(duration/originalDuration))*Mathf.PI));
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
            ToggleAll(false);
            _battleScreen.UpdatePlayerSkillBar(_currentPlayerCharacter);
            ToggleMainMapScreen(true);
        }

        public void LevelSelected(int index)
        {
            if(_gameLevels == null || _gameLevels.Count == 0)
            {
                //Debug.LogWarning("Game Levels: List is null or has zero elements!");
                return;
            }

            if(index >= _gameLevels.Count || index <0)
            {
                //Debug.LogWarning("Game Levels: Index is out of range!");
                return;
            }

            _currentGameLevelIndex = index;
            ToggleAll(false);
            ToggleBattleScreen(true);
            _battleScreen.UpdateBackground(_gameLevels[_currentGameLevelIndex].Background);
            _battleScreen.SetButtonText(BattleControlButtonTextType.END_TURN);
            TogglePlayerCharacter(true);
            _currentEnemyInLevelIndex = 0;
            var enemyTemplateToSpawn = _gameLevels[index].Enemies[_currentEnemyInLevelIndex].Template;
            if(enemyTemplateToSpawn == null)
            {
                //Debug.LogWarning("Level Selected: Enemy template in current level is null!");
                return;
            }

            _currentEnemyCharacter.SetCharacterTemplate(enemyTemplateToSpawn);
 
            //ToggleEnemyCharacter(true);
            //TogglePlayerCharacter(true);
            UpdateResourcesUI();
            GameState = GameState.BATTLE_PLAYER_TURN;
        }

        private void UpdateCurrentBattleBackground()
        {

        }

        public bool SpawnNextEnemy()
        {
            var enemyList = _gameLevels[_currentGameLevelIndex].Enemies;
            if (enemyList == null || _currentEnemyInLevelIndex >= enemyList.Count-1)
            {
                //Debug.Log("Spawning Next Enemy: No more enemies to spawn.");
                //_battleScreen.SetButtonText(BattleControlButtonTextType.BACK_TO_MAIN_MAP);
                return false;
            }
            StartCoroutine(SpawnEnemyCO(enemyList[_currentEnemyInLevelIndex + 1].Template));
            return true;
        }

        private IEnumerator SpawnEnemyCO(CharacterTemplate characterTemplate)
        {
            CurrentEnemyCharacter().FadeOut(TurnDelay);
            _battleScreen.ToggleEnemyResources(false);
            yield return new WaitForSeconds(TurnDelay);
            _currentEnemyInLevelIndex++;
            _currentEnemyCharacter.EndBattle();
            _currentEnemyCharacter.SetCharacterTemplate(characterTemplate);
            CurrentEnemyCharacter().FadeIn(TurnDelay);
            yield return new WaitForSeconds(TurnDelay);
            _battleScreen.ToggleEnemyResources(true);
            UpdateResourcesUI();
            ChangeGameState(GameState.BATTLE_START);
        }

        public void ReportCharacterDeath(Character character)
        {
            if(character == null)
            {
                //Debug.LogWarning("Report Character Death: Character is null!");
                return;
            }

            if(character == _currentPlayerCharacter)
            {
                //StopAllCoroutines();
                ChangeGameState(GameState.BATTLE_PLAYER_DEFEAT, TurnDelay);
                //_battleScreen.SetButtonText(BattleControlButtonTextType.BACK_TO_MAIN_MAP);
                //player died
            }

            if(character == _currentEnemyCharacter)
            {
                //StopAllCoroutines();
                //Enemy defeated
                ChangeGameState(GameState.BATTLE_PLAYER_WIN, TurnDelay);
                //SpawnNextEnemy();
            }

        }

        private void ToggleMainMenuScreen(bool value)
        {
            ToggleObject(_mainMenuScreen.gameObject, value);
        }
        private void ToggleMainMapScreen(bool value)
        {
            ToggleObject(_mainMapScreen.gameObject, value);
        }
        
        private void ToggleBattleScreen(bool value)
        {
            ToggleObject(_battleScreen.gameObject, value);

            if (value == true)
            {
                _battleScreen.UpdatePlayerResources(_currentPlayerCharacter);
                _battleScreen.UpdateEnemyResources(_currentEnemyCharacter);
            }
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

        public void ShakeCamera(float amount, float duration)
        {
            if(duration <= 0)
            {
                return;
            }
            StartCoroutine(ShakeCameraCO(amount, duration));
        }

        private IEnumerator ShakeCameraCO(float amount, float duration)
        {
            Camera cam = Camera.main;
            float originalDuration = duration;
            Vector3 originalPos = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
            while (duration > 0)
            {
                Vector3 pos = new Vector3(Random.Range(-amount, amount), Random.Range(-amount, amount), cam.transform.position.z);
                //pos = Vector3.Lerp(pos, originalPos, (1-duration) / originalDuration);
                cam.transform.position = pos;
                duration -= Time.deltaTime;
                amount /= 1.5f;
                yield return null;
            }
            cam.transform.position = originalPos;
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
        BATTLE_START,
        BATTLE_PLAYER_TURN,
        BATTLE_ENEMY_TURN,
        BATTLE_PLAYER_DEFEAT,
        BATTLE_PLAYER_WIN,
        END_OF_LEVEL,
        END_SCREEN,
        BATTLE_TRANSITION,
        SCREEN_TRANSITION,
        SHOWING_MESSAGE
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