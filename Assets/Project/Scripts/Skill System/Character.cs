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
        public bool IsPlayer = false;

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

        private int _damageIncreaseNextAbility = 0;
        private int _energyCostReductionNextAbility = 0;
        private Dictionary<Skill, int> _damageIncreaseNextByAbility = new Dictionary<Skill, int>();
        private Dictionary<Skill, int> _energyCostReductionNextByAbility = new Dictionary<Skill, int>();

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

            if(HasEnergyFor(skill, out int energyCost) == false)
            {
                return;
            }

            _energy -= energyCost;
            

            if(skill.BaseDamage > 0)
            {
                target.TakeDamage(CalculateDamageNextAbility(skill),this, gameManager);

            }

            if(skill.DamagePerTurn > 0)
            {

            }


            if(skill.Heal > 0)
            {
                Heal(skill.Heal, gameManager);
            }
            SpawnSkillVFX(skill, gameManager);
            ClearNextBonusAll();
            ClearNextBonusBySkill(skill);
            ApplyNextBonusFromSkill(skill);
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

        public bool HasEnergyFor(Skill skill, out int finalEnergyCost)
        {
            int finalCost = skill.EnergyCost;
            int costReductionNextByAbility = 0;
            if (_energyCostReductionNextByAbility.ContainsKey(skill))
            {
                costReductionNextByAbility = _energyCostReductionNextByAbility[skill];
            }

            finalCost = finalCost - (_energyCostReductionNextAbility * skill.EnergyCost / 100);
            finalCost = finalCost - (costReductionNextByAbility * skill.EnergyCost / 100);
            finalEnergyCost = finalCost;

            if (_energy >= finalCost)
            {
                return true;
            } else
            {
                return false;
            }
            
        }

        private int CalculateDamageNextAbility(Skill skill)
        {
            int result = skill.BaseDamage;
            result += _damageIncreaseNextAbility * skill.BaseDamage / 100;

            if(_damageIncreaseNextByAbility.ContainsKey(skill))
            {
                result += _damageIncreaseNextByAbility[skill] * skill.BaseDamage / 100;

            }
            return result;
        }


        private void ApplyNextBonusFromSkill(Skill skill)
        {
            if(skill == null)
            {
                return;
            }

            if(skill.EnhanceNextSkill == null)
            {
                _damageIncreaseNextAbility += skill.EnhanceNextDamage;
                _energyCostReductionNextAbility += skill.EnhanceNextEnergyCost;
            } else
            {
                if (_damageIncreaseNextByAbility.ContainsKey(skill))
                {
                    _damageIncreaseNextByAbility[skill] += skill.EnhanceNextDamage;
                } else
                {
                    _damageIncreaseNextByAbility.Add(skill, skill.EnhanceNextDamage);
                }

                if (_energyCostReductionNextByAbility.ContainsKey(skill))
                {
                    _energyCostReductionNextByAbility[skill] += skill.EnhanceNextEnergyCost;
                }
                else
                {
                    _energyCostReductionNextByAbility.Add(skill, skill.EnhanceNextEnergyCost);
                }
            }
        }

        private void ClearNextBonusAll()
        {
            _damageIncreaseNextAbility = 0;
            _energyCostReductionNextAbility = 0;
        }

        private void ClearNextBonusBySkill(Skill skill)
        {
            if(_damageIncreaseNextByAbility.ContainsKey(skill))
            {
                _damageIncreaseNextByAbility[skill] = 0;
            }

            if(_energyCostReductionNextByAbility.ContainsKey(skill))
            {
                _energyCostReductionNextByAbility[skill] = 0;
            }
        }

        private void SpawnSkillVFX(Skill skill, GameManager gameManager)
        {
            var skillEnemyVFX = skill.EnemySkillPrefab;
            if (skillEnemyVFX != null)
            {
                var spawnedEffect = Instantiate(skillEnemyVFX, IsPlayer? gameManager.Enemy.transform: gameManager.Player.transform);
                spawnedEffect.transform.parent = null;

            }

            var skillSelfVFX = skill.SelfSkillPrefab;
            if (skillSelfVFX != null)
            {
                var spawnedEffect = Instantiate(skillSelfVFX, transform);
                spawnedEffect.transform.parent = null;

            }

            if (IsPlayer)
            {
                if (skill.AnimationType == PlayerAnimationType.SHIELD)
                {
                    _animator.SetTrigger("Shield");
                }
                else
                {
                    _animator.SetTrigger("Attack");
                }

            }
            else
            {
                _animator.SetTrigger("Attack");
            }
        }
    }

}