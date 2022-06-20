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

        private void Start()
        {
            _player = Player.GetComponent<Character>();
            _enemy = Enemy.GetComponent<Character>();

        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                _player.ActivateSkill(0,_enemy, this);
            }

            UpdateUI();
        }

        public void UpdateUI()
        {
            PlayerHealth.UpdateValue(_player.HealthPercent());
            PlayerEnergy.UpdateValue(_player.EnergyPercent());
            EnemyHealth.UpdateValue(_enemy.HealthPercent());
            EnemyEnergy.UpdateValue(_enemy.EnergyPercent());

        }
    }

}