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
        [Header("Starting Level")]
        public GameLevel StartingLevel;

        [Space(20)]
        [Header("Spawn Locations")]
        public Transform EnemySpawnPoint;
        public Transform PlayerSpawnPoint;
        

        [Space(20)]
        [SerializeField]
        [Header("UI: Canvas")]
        private Canvas _dynamicCanvas;
        [SerializeField]
        private Canvas _staticCanvas;
        public GameObject MainMenuPrefab;

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
        public SliderUI PlayerHealth;
        public SliderUI PlayerEnergy;

        public SliderUI EnemyHealth;
        public SliderUI EnemyEnergy;

        [Space(20)]
        [Header("Floating Combat Text")]
        public GameObject DamageNumberUIPrefab;
        public TextColors TextColorsPreset;

        private bool _playerTurn = true;
        private float _abilityRechargeTime = 1f;
        private int _currentEnemyIndexInLevel = 0;
        private float _turnDelayTime = 1f;

        

        private void Awake()
        {
            /*_currentPlayerCharacter = PlayerPrefab.GetComponent<Character>();
            _currentPlayerCharacter.Initialize(PlayerTemplate, this);
            /*_enemy = Enemy.GetComponent<Character>();
            _enemy.Initialize(this);
            SpawnEnemy();*/
            SpawnCharacter(PlayerTemplate, true);
            SpawnCharacter(StartingEnemyTemplate);

        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                ClearCurrentEnemy();
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
            UpdateUI();
        }

        public void UpdateUI()
        {
            PlayerHealth.UpdateValue(_currentPlayerCharacter.HealthPercent());
            PlayerEnergy.UpdateValue(_currentPlayerCharacter.EnergyPercent());
            EnemyHealth.UpdateValue(_currentEnemyCharacter.HealthPercent());
            EnemyEnergy.UpdateValue(_currentEnemyCharacter.EnergyPercent());

        }
        public void SpawnPlayer(int level=1)
        {
            if (_currentPlayer != null)
            {
                _currentPlayer.SetActive(true);
                if (PlayerTemplate != null)
                {
                    _currentPlayerCharacter.Initialize(PlayerTemplate, this, level, true);
                }
                return;
            }

            var spawn = Instantiate(CharacterPrefab, PlayerSpawnPoint);
            _currentPlayer = spawn;
            _currentPlayerCharacter = spawn.GetComponent<Character>();
            _currentPlayerCharacter.Initialize(PlayerTemplate, this, level, true);
        }

        private void SpawnEnemy(CharacterTemplate template, int level = 1)
        {
            if(_currentEnemy != null)
            {
                _currentEnemy.SetActive(true);
                if(template != null)
                {
                    _currentEnemyCharacter.Initialize(template, this, level, false);
                }
                return;
            }

            var spawn = Instantiate(CharacterPrefab, EnemySpawnPoint);
            _currentEnemy = spawn;
            _currentEnemyCharacter = spawn.GetComponent<Character>();
            _currentEnemyCharacter.Initialize(template, this, level, false);
        }


        public void SpawnCharacter(CharacterTemplate template, bool IsPLayer = false )
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

        public void SpawnNextEnemy()
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

        private void StartGame()
        {
            ShowMainMenu();
        }
        private void ShowMainMenu()
        {

        }

        private void EnterMainMap()
        {

        }

        private void EnterBattle()
        {
            HideMainMenu();
            ShowLoadingScreen();
            SpawnBattleBackground();
            SpawnBattleHUD();
            SpawnPlayer();
            SpawnNextEnemy();

            HideLoadingScreen();
        }

        private void LeaveBattle()
        {

        }

        private void SpawnBattleHUD()
        {

        }

        private void HideHUD()
        {

        }

        private void SpawnBattleBackground()
        {

        }

        private void HideBattleBackground()
        {

        }

        private void HideMainMenu()
        {

        }

        private void ShowLoadingScreen()
        {

        }

        private void HideLoadingScreen()
        {

        }

        private void ShowMap()
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