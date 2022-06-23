using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover
{
    public class GameManager : MonoBehaviour
    {
        public GameObject Player;
        private Character _player;

        public GameObject Enemy;
        private Character _enemy;

        public SliderUI PlayerHealth;
        public SliderUI PlayerEnergy;

        public SliderUI EnemyHealth;
        public SliderUI EnemyEnergy;

        public GameObject DamageNumberUIPrefab;

        public Canvas DynamicCanvas;

        private bool _playerTurn = true;
        private float _abilityRechargeTime = 1f;

        private float _turnDelayTime = 1f;

        private void Awake()
        {
            _player = Player.GetComponent<Character>();
            _player.Initialize(this);
            _enemy = Enemy.GetComponent<Character>();
            _enemy.Initialize(this);

        }
        private void Update()
        {

            HandleAbilityRecharge();
            HandlePlayerInput();
            UpdateUI();
        }

        public void UpdateUI()
        {
            PlayerHealth.UpdateValue(_player.HealthPercent());
            PlayerEnergy.UpdateValue(_player.EnergyPercent());
            EnemyHealth.UpdateValue(_enemy.HealthPercent());
            EnemyEnergy.UpdateValue(_enemy.EnergyPercent());

        }

        public void SpawnDamageNumber(int value, bool AtEnemy = true)
        {
            var spawn = Instantiate(DamageNumberUIPrefab, DynamicCanvas.transform);
            spawn.GetComponent<DamageNumber>().Initialize(value, AtEnemy ? Enemy.transform.position : Player.transform.position);

        }

        public void EndTurn(Character endedByCharacter)
        {
            if (endedByCharacter.IsPlayer == true)
            {
                _playerTurn = false;
                _enemy.EndTurn();
                _enemy.MakeNextMove(_player, this);


            } else
            {
                _player.EndTurn();
                _playerTurn = true;
            }
        }

        public void EndTurnButtonPress()
        {
            EndTurn(_player);
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
                if (_player.ActivateSkill(0, _enemy) == true)
                {
                    TriggerAbilityRecharge();
                }
            }


            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (_player.ActivateSkill(1, _enemy) == true)
                {
                    TriggerAbilityRecharge();
                }
            }


            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (_player.ActivateSkill(2, _enemy) == true)
                {
                    TriggerAbilityRecharge();
                }
            }


            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (_player.ActivateSkill(3, _enemy) == true)
                {
                    TriggerAbilityRecharge();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (_player.ActivateSkill(4, _enemy) == true)
                {
                    TriggerAbilityRecharge();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (_player.ActivateSkill(5, _enemy) == true)
                {
                    TriggerAbilityRecharge();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (_player.ActivateSkill(6, _enemy) == true)
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
    }

}