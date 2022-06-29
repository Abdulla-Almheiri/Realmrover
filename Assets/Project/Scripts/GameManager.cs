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


        public GameObject EnemyPrefab;
        private GameObject _currentEnemy;
        private Character _currentEnemyCharacter;
        public CharacterTemplate StartingEnemyTemplate;

        [Space(20)]
        [Header("Floating Combat Text")]
        public GameObject DamageNumberUIPrefab;
        public TextColors TextColorsPreset;

        private bool _playerTurn = true;
        private float _maxAbilityRechargeTime = 1f;
        private float _abilityRechargeTime = 0.5f;
        private float _turnDelayTime = 1f;
        private GameState _gameState = GameState.MAIN_MENU;
        private bool _gameStateDirty = true;



        private void Awake()
        {
            Initialize();
        }
        private void Update()
        {
            HandleAbilityRecharge();
            HandleGame();
        }

        //To be called first in Awake()
        private void Initialize()
        {
            InitializeCanvases();
            InitializeMainMenuScreen();
            InitializeMainMapScreen();
            InitializeBattleScreen();
            InitializePlayer();
            InitializeEnemy();
            //EnterGame();
        }

        private void Start()
        {
            EnterGame();
        }

        //To be called every frame in Update()
        private void HandleGame()
        {
            HandleCombat();
        }

        private void HandleCombat()
        {
            switch (_gameState)
            {
                case GameState.MAIN_MENU:
                    break;
                case GameState.MAIN_MENU_SETTINGS:
                    break;
                case GameState.MAIN_MAP:
                    break;
                case GameState.BATTLE_START:
                    HandlePlayerInput();
                    break;
                case GameState.BATTLE_PLAYER_TURN:
                    HandlePlayerInput();
                    break;
                case GameState.BATTLE_ENEMY_TURN:
                    HandleEnemyAI();
                    break;
                case GameState.BATTLE_PLAYER_DEFEAT:
                    break;
                case GameState.BATTLE_PLAYER_WIN:
                    if(SpawnNextEnemy())
                    {
                        ChangeGameState(GameState.BATTLE_PLAYER_TURN, _turnDelayTime);

                    } else
                    {
                        ChangeGameState(GameState.END_OF_LEVEL, _turnDelayTime);
                    }
                    break;
                case GameState.END_SCREEN:
                    break;
                case GameState.TRANSITION:
                    break;
                default:
                    break;
            }
        }

        private void HandleEnemyAI()
        {
            _currentEnemyCharacter.MakeNextMove(_currentPlayerCharacter);
            EndTurn(_currentEnemyCharacter);
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

        public void SpawnDamageNumber(int value, FloatingTextType textType, bool AtEnemy = true)
        {
            if (EnemyPrefab == null || PlayerPrefab == null || _currentEnemyCharacter == null || _currentPlayerCharacter == null)
            {
                return;
            }
            var spawn = Instantiate(DamageNumberUIPrefab, _dynamicCanvas.transform);
            var damageNumber = spawn.GetComponent<DamageNumber>();
            damageNumber.Initialize(value, textType, AtEnemy ? _currentEnemy.transform.position : _currentPlayer.transform.position, this);

        }

        public void EndTurn(Character endedByCharacter)
        {
            if(endedByCharacter == _currentPlayerCharacter)
            {
                _currentPlayerCharacter.EndTurn();
                ChangeGameState(GameState.BATTLE_ENEMY_TURN, _turnDelayTime);
            } else if(endedByCharacter == _currentEnemyCharacter)
            {
                _currentEnemyCharacter.EndTurn();
                ChangeGameState(GameState.BATTLE_PLAYER_TURN, _turnDelayTime);
            }

            UpdateResourcesUI();
        }

        public void UpdateResourcesUI()
        {
            _battleScreen.UpdatePlayerResources(_currentPlayerCharacter);
            _battleScreen.UpdateEnemyResources(_currentEnemyCharacter);

        }
        public void EndTurnButtonPress()
        {
            if (_gameState == GameState.TRANSITION)
            {
                return;
            }

            if (_gameState == GameState.BATTLE_PLAYER_TURN)
            {
                EndTurn(_currentPlayerCharacter);
            } else if(_gameState == GameState.END_OF_LEVEL)
            {
                ToggleAll(false);
                ToggleMainMapScreen(true);
                _currentPlayerCharacter.EndBattle();
                _currentEnemyCharacter.EndBattle();
            } else if(_gameState == GameState.BATTLE_PLAYER_DEFEAT)
            {
                ToggleAll(false);
                ToggleMainMapScreen(true);
                _currentPlayerCharacter.EndBattle();
                _currentEnemyCharacter.EndBattle();
            }


        }
        public void ActivateAbility(int index)
        {
            if (_gameState != GameState.BATTLE_PLAYER_TURN)
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
            if (_gameState != GameState.BATTLE_PLAYER_TURN )
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

        private void ChangeGameState(GameState newState, float delay = 0f)
        {
            if(delay == 0f)
            {
                _gameState = newState;
                return;
            }
            StartCoroutine(SwitchStateDelayCO( newState, delay));
        }

        private IEnumerator SwitchStateDelayCO(GameState newState, float delay)
        {
            _gameState = GameState.TRANSITION;
            yield return new WaitForSeconds(delay);
            _gameState = newState;
        }
        public void BattleControlButtonPressed(BattleButtonStates buttonState)
        {
            switch (buttonState)
            {
                case BattleButtonStates.END_TURN:
                    //EndTurnButtonPress();
                    break;
                case BattleButtonStates.NEXT_BATTLE:
                    break;
                case BattleButtonStates.LEAVE:
                    break;
                default:
                    break;
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
                Debug.LogWarning("Game Levels: List is null or has zero elements!");
                return;
            }

            if(index >= _gameLevels.Count || index <0)
            {
                Debug.LogWarning("Game Levels: Index is out of range!");
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
                Debug.LogWarning("Level Selected: Enemy template in current level is null!");
                return;
            }

            _currentEnemyCharacter.SetCharacterTemplate(enemyTemplateToSpawn);
 
            ToggleEnemyCharacter(true);
            UpdateResourcesUI();
            _gameState = GameState.BATTLE_PLAYER_TURN;
        }

        private void UpdateCurrentBattleBackground()
        {

        }

        public bool SpawnNextEnemy()
        {
            var enemyList = _gameLevels[_currentGameLevelIndex].Enemies;
            if (enemyList == null || _currentEnemyInLevelIndex >= enemyList.Count-1)
            {
                Debug.Log("Spawning Next Enemy: No more enemies to spawn.");
                _battleScreen.SetButtonText(BattleControlButtonTextType.BACK_TO_MAIN_MAP);
                return false;
            }
            
            _currentEnemyInLevelIndex++;
            _currentEnemyCharacter.EndBattle();
            _currentEnemyCharacter.SetCharacterTemplate(enemyList[_currentEnemyInLevelIndex].Template, enemyList[_currentEnemyInLevelIndex].Level);
            UpdateResourcesUI();
            return true;
        }

        public void ReportCharacterDeath(Character character)
        {
            if(character == null)
            {
                Debug.LogWarning("Report Character Death: Character is null!");
                return;
            }

            if(character == _currentPlayerCharacter)
            {
                StopAllCoroutines();
                ChangeGameState(GameState.BATTLE_PLAYER_DEFEAT);
                _battleScreen.SetButtonText(BattleControlButtonTextType.BACK_TO_MAIN_MAP);
                //player died
            }

            if(character == _currentEnemyCharacter)
            {
                StopAllCoroutines();
                //Enemy defeated
                ChangeGameState(GameState.BATTLE_PLAYER_WIN);
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
        }
        private void ToggleAll(bool value)
        {
            ToggleMainMapScreen(value);
            ToggleMainMenuScreen(value);
            ToggleBattleScreen(value);
            TogglePlayerCharacter(value);
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
        TRANSITION
    }
}