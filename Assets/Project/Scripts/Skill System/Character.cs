using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover
{
    public class Character : MonoBehaviour
    {
        public CharacterTemplate CharacterTemplate = null;
        private int _level = 1;
        private bool _alive = false;
        public bool Is_Player = false;

        private int _maxHealthPerLevel = 0;
        private int _maxEnergyPerLevel = 0;
        private int _HealthRegenPerLevel = 0;
        private int _EnergyRegenPerLevel = 0;

        private int _currentHealth = 0;
        private int _maxHealth = 0;
        private int _healthRegen = 0;
        private int _energy = 0;
        private int _maxEnergy = 0;
        private int _energyRegen = 0;

        private int Absorb = 0;
        private int Reflect = 0;

        private List<Skill> _skills = new List<Skill>();

        private Animator _animator;
        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Initialize(bool updateStatsPerLevel = false)
        {
            if(CharacterTemplate == null)
            {
                return;
            }

            _alive = true;

            _maxHealthPerLevel = CharacterTemplate.MaxHealthPerLevel;
            _maxEnergyPerLevel = CharacterTemplate.MaxHealthPerLevel;
            _EnergyRegenPerLevel = CharacterTemplate.EnergyRegenPerLevel;
            _HealthRegenPerLevel = CharacterTemplate.HealthRegenPerLevel;

            _level = CharacterTemplate.Level;
            _currentHealth = CharacterTemplate.Health + (updateStatsPerLevel? _level-1: 0)* _maxHealthPerLevel;
            _maxHealth = _currentHealth;
            _healthRegen = CharacterTemplate.HealthRegen + (updateStatsPerLevel ? _level - 1 : 0) * _HealthRegenPerLevel;
            _energy = CharacterTemplate.Energy + (updateStatsPerLevel ? _level - 1 : 0) * _maxEnergyPerLevel;
            _maxEnergy = _energy;
            _energyRegen = CharacterTemplate.EnergyRegen + (updateStatsPerLevel ? _level - 1 : 0) * _EnergyRegenPerLevel;

            _skills = new List<Skill>(CharacterTemplate.Skills);

            _animator = GetComponent<Animator>();
        }

        public void ActivateSkill(int index, Character target, GameManager gameManager)
        {
            var skill = _skills[index];
            if(skill == null)
            {
                return;
            }

            if(skill.Damage > 0)
            {
                target.TakeDamage(skill.Damage,this, gameManager);

            }

            if(skill.DamagePerTurn > 0)
            {

            }


            if(skill.Heal > 0)
            {
                Heal(skill.Heal, gameManager);
            }

            //spawn vfx
            var skillEnemyVFX = skill.EnemySkillPrefab;
            if(skillEnemyVFX != null)
            {
                var spawnedEffect = Instantiate(skillEnemyVFX, gameManager.Enemy.transform);
                spawnedEffect.transform.parent = null;

            }

            var skillSelfVFX = skill.SelfSkillPrefab;
            if (skillSelfVFX != null)
            {
                var spawnedEffect = Instantiate(skillSelfVFX, transform);
                spawnedEffect.transform.parent = null;

            }

            if(Is_Player)
            {
                if (skill.AnimationType == PlayerAnimationType.SHIELD)
                {
                    _animator.SetTrigger("Shield");
                } else
                {
                    _animator.SetTrigger("Attack");
                }

            } else
            {
                _animator.SetTrigger("Attack");
            }

        }

        public void ApplyEffectOverTime(Skill skill, GameManager gameManager)
        {

        }

        public void TakeDamage(int amount, Character attacker, GameManager gameManager)
        {
            if(_alive == false)
            {
                return;
            }

            if(Absorb > 0)
            {
                Absorb -= amount;
                if(Absorb < 0)
                {
                    _currentHealth += Absorb;
                    Absorb = 0;
                    CheckDead();
                }
            }

            _currentHealth -= amount;
            Debug.Log(amount + " damage taken by " + this.CharacterTemplate.name + ". Health is : " + _currentHealth + "/" + _maxHealth);
            _animator.SetTrigger("Hit");
            CheckDead();
        }

        private void CheckDead()
        {
            if(_currentHealth <= 0)
            {
                _alive = false;
                _currentHealth = 0;
                _animator.SetTrigger("Death");
            }
        }

        public void Heal(int amount, GameManager gameManager)
        {
            _currentHealth += amount;
            if(_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
        }

        public bool IsAlive()
        {
            return _alive;
        }

        public float HealthPercent()
        {
            return (float)_currentHealth / (float)_maxHealth;
        }

        public float EnergyPercent()
        {
            return (float)_energy / (float)_maxEnergy;
        }
    }

}