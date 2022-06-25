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
        private int _currentEnemyInLevelIndex = 0;

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

        public GameObject BattleBackgroundPrefab;
        private BattleBackgroundScript _battleBackground;

        [Space(20)]
        [Header("Floating Combat Text")]
        public GameObject DamageNumberUIPrefab;
        public TextColors TextColorsPreset;

        private bool _playerTurn = true;
        private float _abilityRechargeTime = 1f;
        private float _turnDelayTime = 1f;

        

        private void Awake()
        {
            Initialize();
            /*_currentPlayerCharacter = PlayerPrefab.GetComponent<Character>();
            _currentPlayerCharacter.Initialize(PlayerTemplate, this);
            /*_enemy = Enemy.GetComponent<Character>();
            _enemy.Initialize(this);
            SpawnEnemy();*/


        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                EndTurnButtonPress();
            }

            //test
            if (Input.GetKeyDown(KeyCode.X))
            {
                _currentEnemyCharacter.gameObject.GetComponent<Animator>().runtimeAnimatorController = seraphim_anim;
                //Enemy.GetComponent<SpriteRenderer>().sprite = seraphim_sprite;

                Debug.Log("Animation Controller is:   " + EnemyPrefab.GetComponent<Animator>().name);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                _currentEnemyCharacter.gameObject.GetComponent<Animator>().runtimeAnimatorController = titanoboa_anim;
                //Enemy.GetComponent<SpriteRenderer>().sprite = titanoboa_sprite;
            }

            HandleAbilityRecharge();
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

        public void UpdateUI()
        {
            _playerHUD.UpdateStats();
        }

        public void StatsChanged()
        {
            UpdateUI();
        }
        public void SpawnPlayer(int level=1)
        {
            if (_currentPlayer != null)
            {
                _currentPlayer.SetActive(true);
                Debug.Log("player set to active");
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
                if (_currentPlayerCharacter.ActivateSkill(0, _currentEnemyCharacter) == true)
                {
                    TriggerAbilityRecharge();
                }
            }


            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (_currentPlayerCharacter.ActivateSkill(1, _currentEnemyCharacter) == true)
                {
                    TriggerAbilityRecharge();
                }
            }


            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (_currentPlayerCharacter.ActivateSkill(2, _currentEnemyCharacter) == true)
                {
                    TriggerAbilityRecharge();
                }
            }


            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (_currentPlayerCharacter.ActivateSkill(3, _currentEnemyCharacter) == true)
                {
                    TriggerAbilityRecharge();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (_currentPlayerCharacter.ActivateSkill(4, _currentEnemyCharacter) == true)
                {
                    TriggerAbilityRecharge();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (_currentPlayerCharacter.ActivateSkill(5, _currentEnemyCharacter) == true)
                {
                    TriggerAbilityRecharge();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (_currentPlayerCharacter.ActivateSkill(6, _currentEnemyCharacter) == true)
                {
                    TriggerAbilityRecharge();
                }
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

        public void SpawnNextEnemy(GameLevel gameLevel)
        {
            if (_currentEnemyInLevelIndex >= gameLevel.Enemies.Count)
            {
                //Handle Next Level here
                return;
            }
            else
            {
                SpawnCharacter(gameLevel.Enemies[_currentEnemyInLevelIndex].Template, false, gameLevel.Enemies[_currentEnemyInLevelIndex].Level);
            }
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
            ShowMainMenu();
        }

        public void StartGameSession()
        {
            ShowMainMap();
            HideMainMenu();
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
            EnterBattle(Levels[index]);

        }
        private void EnterBattle(GameLevel gameLevel)
        {
            ShowLoadingScreen();
            HideMainMap();
            ShowBattleBackground(gameLevel);
            SpawnPlayer();
            SpawnNextEnemy(gameLevel);
            ShowBattleHUD();


            HideLoadingScreen();
        }

        private void LeaveBattle()
        {
            HideBattleBackground();
            HideBattleHUD();
            EnterMainMap();
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

        private void ShowBattleBackground(GameLevel gameLevel)
        {
            if (_battleBackground != null)
            {
                _battleBackground.gameObject.SetActive(true);
            }
            else
            {
                _battleBackground = Instantiate(BattleBackgroundPrefab).GetComponent<BattleBackgroundScript>();
                _battleBackground.Initialize(this);
                _battleBackground.SetBackground(gameLevel);
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

        private void HideAll()
        {

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
        BATTLE_START,
        PLAYER_TURN,
        ENEMY_TURN,
        LOSS,
        VICTORY,
        INGAME_SETTINGS,
        END_SCREEN


    }
}