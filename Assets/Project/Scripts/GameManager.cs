using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Realmrover
{
    public class GameManager : MonoBehaviour
    {
        public GameState GameState { get; private set; } = GameState.MAIN_MENU;

        [Header("Character Prefab")]
        public GameObject CharacterPrefab;

        [Space(20)]
        [Header("Player Information")]
        public GameObject PlayerPrefab;
        private GameObject _currentPlayer;
        private Character _currentPlayerCharacter;
        public CharacterTemplate PlayerTemplate;

        [Space(20)]
        [Header("Main Menu")]
        public GameObject MainMenuPrefab;
        private MainMenuScript _mainMenu;

        [Space(20)]
        [Header("Main Map")]
        public GameObject MainMapPrefab;
        private MainMapScript _mainMap;

        [Space(20)]
        [Header("Levels")]
        public List<GameLevel> Levels = new List<GameLevel>();
        private int _currentEnemyInLevelIndex = -1;
        private GameLevel _currentGameLevel;

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


        public AnimatorOverrideController seraphim_anim;
        public Sprite seraphim_sprite;
        public AnimatorOverrideController titanoboa_anim;
        public Sprite titanoboa_sprite;



        public GameObject EnemyPrefab;
        private GameObject _currentEnemy;
        private Character _currentEnemyCharacter;
        public CharacterTemplate StartingEnemyTemplate;

        [Space(20)]
        [Header("Battle HUD")]
        public GameObject PlayerHUDPrefab;
        private CharacterHUDScript _playerHUD;

        public GameObject SkillBarPrefab;
        private SkillBarUIScript _skillBar;

        public GameObject BattleBackgroundPrefab;
        private BattleBackgroundScript _battleBackground;

        public GameObject BattleControlButtonPrefab;
        private BattleControlButtonScript _battleControlButton;

        [Space(20)]
        [Header("Floating Combat Text")]
        public GameObject DamageNumberUIPrefab;
        public TextColors TextColorsPreset;

        private bool _playerTurn = true;
        private float _maxAbilityRechargeTime = 1f;
        private float _abilityRechargeTime = 1f;
        private float _turnDelayTime = 1f;
        private GameState _gameState = GameState.MAIN_MENU;
        private bool _gameStateDirty = true;
        

        private void Awake()
        {
            Initialize();
        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                if(IsRecharging() == false)
                {
                    EndTurnButtonPress();
                }

            }

            //test
            if (Input.GetKeyDown(KeyCode.X))
            {
                _currentEnemyCharacter.gameObject.GetComponent<Animator>().runtimeAnimatorController = seraphim_anim;
                //Enemy.GetComponent<SpriteRenderer>().sprite = seraphim_sprite;

                
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                _currentEnemyCharacter.gameObject.GetComponent<Animator>().runtimeAnimatorController = titanoboa_anim;
                //Enemy.GetComponent<SpriteRenderer>().sprite = titanoboa_sprite;
            }

            HandleAbilityRecharge();
            UpdateSkillBarRecharge();
            HandlePlayerInput();
            //UpdateUI();
        }

        
        private void Initialize()
        {
            //SpawnCharacter(PlayerTemplate, true);
            //SpawnCharacter(StartingEnemyTemplate);
            InitializeCanvases();
            //InitializeBattleHUD();

            StartGame();
        }

        private void UpdateSkillBarRecharge()
        {
            if (_skillBar == null || _skillBar.gameObject.activeSelf == false)
            {
                return;
            }

            _skillBar.UpdateRechargeAllSkills(_abilityRechargeTime/_maxAbilityRechargeTime);
        }
        private void InitializeCanvases()
        {
            _dynamicCanvas = Instantiate(DynamicCanvasPrefab);
            _dynamicCanvas.worldCamera = Camera.main;
            _staticCanvas = Instantiate(StaticCanvasPrefab);
            _staticCanvas.worldCamera = Camera.main;
        }
        private void InitializeBattleHUD()
        {
            _playerHUD = Instantiate(PlayerHUDPrefab, _staticCanvas.transform).GetComponent<CharacterHUDScript>();
            _playerHUD.Initialize(this, _currentPlayerCharacter);
        }

        private void ChangeGameState(GameState newGameState)
        {
            _gameState = newGameState;
            _gameStateDirty = true;
        }

        private void UpdateGameState()
        {
            if(_gameStateDirty == false)
            {
                return;
            }

            if(_gameState == GameState.MAIN_MENU)
            {
                HideAll();
                ShowMainMenu();
                return;
            }

            if (_gameState == GameState.MAIN_MAP)
            {
                HideAll();
                ShowMainMap();
                return;
            }

            if (_gameState == GameState.BATTLE)
            {
                HideAll();
                ShowBattleScreen();
                return;
            }
        }

        private void HandleMainMenu()
        {
            UpdateGameState(GameState.MAIN_MENU);

        }

        private void HandleMainMap()
        {

        }

        private void HandleBattleScreen()
        {

        }

        private void HandleGameStates()
        {

        }
        public void UpdateGameState(GameState newGameState)
        {
            _gameState = newGameState;
            _gameStateDirty = true;
        }

        private void LoadLevel(GameLevel gameLevel)
        {
            _currentGameLevel = gameLevel;
        }
        public void UpdatePlayerHUD()
        {
            _playerHUD.UpdateStats();
        }

        public void UpdateBattleControlButton(BattleButtonStates buttonState)
        {
            _battleControlButton.UpdateUI(buttonState);
        }
        public void StatsChanged()
        {
            UpdatePlayerHUD();
        }
        public void SpawnPlayer(int level=1)
        {
            if (_currentPlayer != null)
            {
                _currentPlayer.SetActive(true);
            }
            else
            {

                var spawn = Instantiate(CharacterPrefab, PlayerSpawnPoint);
                _currentPlayer = spawn;
                _currentPlayerCharacter = spawn.GetComponent<Character>();
                _currentPlayerCharacter.Initialize(PlayerTemplate, this, level, true);
            }
        }

        private void SpawnEnemy(CharacterTemplate template, int level = 1)
        {
            if (_currentEnemy != null)
            {
                _currentEnemy.SetActive(true);
            }
            else
            {

                var spawn = Instantiate(CharacterPrefab, EnemySpawnPoint);
                _currentEnemy = spawn;
                _currentEnemyCharacter = spawn.GetComponent<Character>();
                _currentEnemyCharacter.Initialize(template, this, level, false);
            }
        }


        public void SpawnCharacter(CharacterTemplate template, bool IsPLayer = false, int level = 1)
        {
            if(template == null)
            {
                return;
            }

            if(IsPLayer == true)
            {
                SpawnPlayer();
            } else
            {
                SpawnEnemy(template);
            }

        }

        public void CharacterDead(Character character)
        {
            if(character == _currentEnemyCharacter)
            {
                CurrentEnemyDefeated();
            } else if(character == _currentPlayer)
            {
                PlayerDefeated();
            }
        }

        private void PlayerDefeated()
        {
            ShowMainMenu();
        }
        public void SpawnDamageNumber(int value, FloatingTextType textType, bool AtEnemy = true)
        {
            if(EnemyPrefab == null || PlayerPrefab == null || _currentEnemyCharacter == null || _currentPlayerCharacter == null)
            {
                return;
            }
            var spawn = Instantiate(DamageNumberUIPrefab, _dynamicCanvas.transform);
            var damageNumber = spawn.GetComponent<DamageNumber>();
            damageNumber.Initialize(value, textType, AtEnemy ? _currentEnemy.transform.position : _currentPlayer.transform.position, this);

        }

        public void EndTurn(Character endedByCharacter)
        {
            if (endedByCharacter.IsPlayer == true)
            {
                _playerTurn = false;
                _currentEnemyCharacter.EndTurn();
                _currentEnemyCharacter.MakeNextMove(_currentPlayerCharacter, this);


            } else
            {
                _currentPlayerCharacter.EndTurn();
                _playerTurn = true;
            }

            StatsChanged();
        }

        public void EndTurnButtonPress()
        {
            EndTurn(_currentPlayerCharacter);
        }
        public void ActivateAbility(int index)
        {
            if (_playerTurn != true)
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
            if (_playerTurn != true)
            {
                return;
            }

            if(IsRecharging())
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

        public void ApplyDelay(float duration)
        {

        }

        public bool SpawnNextEnemy()
        {
            
            if (_currentEnemyInLevelIndex >= _currentGameLevel.Enemies.Count-1)
            {
                //Handle Next Level here
                EndLevel();
                return false;
            }
            else
            {
                _playerTurn = true;
                _currentEnemyInLevelIndex++;
                Debug.Log("NEXT ENEMY!!");
                HideCharacter(_currentEnemyCharacter);
                SpawnCharacter(_currentGameLevel.Enemies[_currentEnemyInLevelIndex].Template, false, _currentGameLevel.Enemies[_currentEnemyInLevelIndex].Level);
                return true;
            }
        }

        public void EndLevel()
        {
            LeaveBattle();
        }

        private void ShowBattleControlButton()
        {
            if (_battleControlButton != null)
            {
                _battleControlButton.gameObject.SetActive(true);
            }
            else
            {
                _battleControlButton = Instantiate(BattleControlButtonPrefab, _staticCanvas.transform).GetComponent<BattleControlButtonScript>();
                _battleControlButton.Initialize(this);
            }
        }
        public void BattleControlButtonPressed()
        {

        }

        private void ClearCurrentEnemy()
        {
            Destroy(EnemyPrefab);
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

        public void StartGame()
        {
            HideAll();
            ShowMainMenu();
        }

        public void StartGameSession()
        {
            HideAll();
            ShowMainMap();
        }

        private void ShowMainMenu()
        {
            if(_mainMenu != null)
            {
                _mainMenu.gameObject.SetActive(true);
            } else
            {
                _mainMenu = Instantiate(MainMenuPrefab, _staticCanvas.transform).GetComponent<MainMenuScript>();
                _mainMenu.Initialize(this);
            }
        }

        private void EnterMainMap()
        {
            HideMainMenu();
            ShowMainMap();
        }

        public void StartLevel(int index)
        {
            if(Levels == null || Levels.Count <= index)
            {
                return;
            }
            _currentGameLevel = Levels[index];
            _currentEnemyInLevelIndex = -1;
            EnterBattle(Levels[index]);

        }
        private void EnterBattle(GameLevel gameLevel)
        {
            HideAll();
            ShowBattleScreen();

            /*ShowLoadingScreen();
            HideMainMap();
            ShowBattleBackground(gameLevel);
            SpawnPlayer();
            SpawnNextEnemy(gameLevel);
            ShowBattleHUD();
            ShowBattleControlButton();*/


            HideLoadingScreen();
        }

        public void CurrentEnemyDefeated()
        {
           if(SpawnNextEnemy())
            {
                return;
            } else
            {
                _currentEnemy = null;
                _currentEnemyCharacter = null;
                ShowMainMap();
            }
        }
        private void LeaveBattle()
        {
            HideBattleBackground();
            HideBattleHUD();
            HideCharacter(_currentPlayerCharacter);
            HideCharacter(_currentEnemyCharacter);
            HideBattleControlButton();
            EnterMainMap();
        }

        private void HideBattleControlButton()
        {
            if(_battleControlButton == null)
            {
                return;
            }

            _battleControlButton.gameObject.SetActive(false);
        }
        private void HideCharacter(Character character)
        {
            if(character == null)
            {
                return;
            }

            character.gameObject.SetActive(false);
        }

        private void ShowBattleHUD()
        {
            if (_playerHUD != null)
            {
                _playerHUD.gameObject.SetActive(true);
            }
            else
            {
                _playerHUD = Instantiate(PlayerHUDPrefab, _staticCanvas.transform).GetComponent<CharacterHUDScript>();
                _playerHUD.Initialize(this, _currentPlayerCharacter);
            }
        }

        private void HideBattleHUD()
        {

        }

        private void ShowBattleBackground()
        {
            if (_battleBackground != null)
            {
                _battleBackground.gameObject.SetActive(true);
            }
            else
            {
                _battleBackground = Instantiate(BattleBackgroundPrefab).GetComponent<BattleBackgroundScript>();
                _battleBackground.Initialize(this);
                _battleBackground.SetBackground(_currentGameLevel);
            }
        }

        private void HideBattleBackground()
        {
            if (_battleBackground == null)
            {
                return;
            }

            _battleBackground.gameObject.SetActive(false);
        }

        private void HideMainMenu()
        {
            if(_mainMenu == null)
            {
                return;
            }

            _mainMenu.gameObject.SetActive(false);
        }

        private void ShowLoadingScreen()
        {

        }

        private void HideLoadingScreen()
        {

        }

        private void ShowMainMap()
        {
            if (_mainMap != null)
            {
                _mainMap.gameObject.SetActive(true);
            }
            else
            {
                _mainMap = Instantiate(MainMapPrefab, _staticCanvas.transform).GetComponent<MainMapScript>();
                _mainMap.Initialize(this);
            }
        }
        private void HideMainMap()
        {
            if (_mainMap == null)
            {
                return;
            }

            _mainMap.gameObject.SetActive(false);
        }
        private void ShowBattleScreen()
        {
            ShowLoadingScreen();
            HideMainMap();
            ShowBattleBackground();
            SpawnPlayer();
            SpawnNextEnemy();
            ShowBattleHUD();
            ShowSkillBar();
            ShowBattleControlButton();

        }
        private void HideBattleScreen()
        {
            HideBattleControlButton();
            ShowLoadingScreen();
            HideMainMap();
            HideBattleBackground();
            HideBattleHUD();
            HideSkillBar();

            //HideLoadingScreen();
        }
        private void HideAll()
        {
            HideBattleScreen();
            HideMainMenu();
            HideMainMap();
        }

        private void ShowSkillBar()
        {
            if(_skillBar != null)
            {
                _skillBar.gameObject.SetActive(true);
            } else
            {
                _skillBar = Instantiate(SkillBarPrefab).GetComponent<SkillBarUIScript>();
                _skillBar.Initialize(this);
                _skillBar.UpdateSkillUI();
            }

        }

        private void HideSkillBar()
        {
            if(_skillBar == null)
            {
                return;
            }

            _skillBar.gameObject.SetActive(false);
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
        MAIN_MAP,
        BATTLE,
        END_SCREEN


    }
}