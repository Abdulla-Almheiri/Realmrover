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
        private GameManager _gameManager;

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

        private int _absorb = 0;
        private int _reflect = 0;

        private List<Skill> _skills = new List<Skill>();

        private int _damageIncreaseNextAbility = 0;
        private int _energyCostReductionNextAbility = 0;
        private Dictionary<Skill, int> _damageIncreaseNextByAbility = new Dictionary<Skill, int>();
        private Dictionary<Skill, int> _energyCostReductionNextByAbility = new Dictionary<Skill, int>();

        private Dictionary<Skill, EffectOverTurns> _healOverTimeEffects = new Dictionary<Skill, EffectOverTurns>();
        private Dictionary<Skill, EffectOverTurns> _IncreaseDamageForTurnsEffects = new Dictionary<Skill, EffectOverTurns>();
        private Dictionary<Skill, EffectOverTurns> _reduceDamageTakenForTurnsEffects = new Dictionary<Skill, EffectOverTurns>();
        private Dictionary<Skill, EffectOverTurns> _reduceEnergyCostForTurnsEffects = new Dictionary<Skill, EffectOverTurns>();

        private Dictionary<Skill, EffectOverTurns> _damageOverTimeEffects = new Dictionary<Skill, EffectOverTurns>();
        private Dictionary<Skill, EffectOverTurns> _damageTakenIncreaseEffects = new Dictionary<Skill, EffectOverTurns>();

        private Animator _animator;
        // Start is called before the first frame update

        private void Awake()
        {
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Initialize(GameManager gameManager, bool updateStatsPerLevel = false)
        {
            _gameManager = gameManager;
            if (CharacterTemplate == null)
            {
                return;
            }

            _alive = true;

            _maxHealthPerLevel = CharacterTemplate.MaxHealthPerLevel;
            _maxEnergyPerLevel = CharacterTemplate.MaxHealthPerLevel;
            _EnergyRegenPerLevel = CharacterTemplate.EnergyRegenPerLevel;
            _HealthRegenPerLevel = CharacterTemplate.HealthRegenPerLevel;

            _level = CharacterTemplate.Level;
            _currentHealth = CharacterTemplate.Health + (updateStatsPerLevel ? _level - 1 : 0) * _maxHealthPerLevel;
            _maxHealth = _currentHealth;
            _healthRegen = CharacterTemplate.HealthRegen + (updateStatsPerLevel ? _level - 1 : 0) * _HealthRegenPerLevel;
            _energy = CharacterTemplate.Energy + (updateStatsPerLevel ? _level - 1 : 0) * _maxEnergyPerLevel;
            _maxEnergy = _energy;
            _energyRegen = CharacterTemplate.EnergyRegen + (updateStatsPerLevel ? _level - 1 : 0) * _EnergyRegenPerLevel;

            _skills = new List<Skill>(CharacterTemplate.Skills);

            _animator = GetComponent<Animator>();
        }

        public bool ActivateSkill(int index, Character target)
        {
            if (IsAlive() == false)
            {
                return false;
            }

            var skill = _skills[index];

            if (skill == null)
            {
                return false;
            }

            if (HasEnergyFor(skill, out int energyCost) == false)
            {
                return false;
            }

            _energy -= energyCost;
            ClearNextEnergyBonusAll();
            ClearNextEnergyBonusBySkill(skill);

            if (skill.BaseDamage > 0)
            {
                target.TakeDamage(CalculateDamageNextAbility(skill, this, target), this, _gameManager);
                ClearNextDamageBonusAll();
                ClearNextDamageBonusBySkill(skill);

            }

            if (skill.Absorb > 0)
            {
                ApplyAbsorbFromSkill(skill);
            }

            if (skill.ReflectDamage > 0)
            {
                ApplyReflectFromSkill(skill);
            }

            if (skill.DamagePerTurn > 0)
            {

            }


            if (skill.Heal > 0)
            {
                Heal(skill.Heal);
            }

            if(skill.HealPerTurn > 0)
            {
                ApplyHealOverTimeEffect(skill);
            }
            SpawnSkillVFX(skill, _gameManager);


            ApplyNextBonusFromSkill(skill);
            return true;
        }

        public void ApplyEffectsOverTime(Skill skill, GameManager gameManager)
        {

        }

        private void ApplyHealOverTimeEffect(Skill skill)
        {
            if(_healOverTimeEffects.ContainsKey(skill))
            {
                if(skill.HealOverTimeStacks == false)
                {
                    if (skill.HealOverTimeRefreshesDuration == true)
                    {
                        _healOverTimeEffects[skill].Turns = skill.HealTurns;
                    }

                    return;
                } else
                {
                    _healOverTimeEffects[skill].Amount += skill.HealPerTurn;

                }
            } else
            {
                _healOverTimeEffects.Add(skill, new EffectOverTurns(skill.HealPerTurn, skill.HealTurns, skill));
            }
        }
        private void TriggerAllEffectsOverTime()
        {
            TriggerHealOverTimeEffects();
        }

        private void TriggerHealOverTimeEffects()
        {
            if(_healOverTimeEffects.Count == 0)
            {
                Debug.Log("NO HEAL OVER TIME ADDED");
                return;
            }
            var values = _healOverTimeEffects.Values;
            foreach (EffectOverTurns effect in values)
            {
                if(effect.Turns == 0 )
                {
                    _healOverTimeEffects.Remove(effect.Source);
                    continue;
                }
                Heal(effect.Amount);
                effect.Turns--;
            }

        }
        public void TakeDamage(int amount, Character attacker, bool ignoreReflect = false)
        {
            if (IsAlive() == false)
            {
                return;
            }

            if (_reflect > 0 && ignoreReflect == false)
            {
                if (_reflect >= amount)
                {
                    attacker.TakeDamage(amount, this, true);
                    _reflect -= amount;
                } else
                {
                    attacker.TakeDamage(_reflect, this, true);
                    _reflect = 0;
                }

            }

            if (_absorb > 0)
            {
                int originalAmount = amount;
                amount -= _absorb;
                if (amount >= 0)
                {
                    _absorb = 0;
                }
                else
                {
                    _absorb -= originalAmount;
                    amount = 0;
                }
            }

            _currentHealth -= amount;
            Debug.Log(amount + " damage taken by " + this.CharacterTemplate.name + ". Health is : " + _currentHealth + "/" + _maxHealth);
            _gameManager.SpawnDamageNumber(amount, attacker.IsPlayer);
            _animator.SetTrigger("Hit");
            CheckDead();
        }

        private void CheckDead()
        {
            if (_currentHealth <= 0)
            {
                _alive = false;
                _currentHealth = 0;
                _animator.SetTrigger("Death");
            }
        }

        public void Heal(int amount)
        {
            _gameManager.SpawnDamageNumber(amount, false);
            _currentHealth += amount;

            if (_currentHealth > _maxHealth)
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

        private int CalculateDamageNextAbility(Skill skill, Character caster, Character receiver)
        {
            int result = skill.BaseDamage;
            result += AttributeBasedAddedSkillDamage(skill, caster, receiver);
            result += _damageIncreaseNextAbility * skill.BaseDamage / 100;

            if (_damageIncreaseNextByAbility.ContainsKey(skill))
            {
                result += _damageIncreaseNextByAbility[skill] * skill.BaseDamage / 100;

            }
            return result;
        }

        private void ApplyNextBonusFromSkill(Skill skill)
        {
            if (skill == null)
            {
                return;
            }

            if (skill.EnhanceNextSkill == null)
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
        private void ApplyAbsorbFromSkill(Skill skill)
        {
            _absorb += skill.Absorb;
            Debug.Log("Absorb is now " + _absorb);
        }

        private void ApplyReflectFromSkill(Skill skill)
        {
            _reflect += skill.ReflectDamage;
        }

        private void ClearNextDamageBonusAll()
        {
            _damageIncreaseNextAbility = 0;
        }

        private void ClearNextEnergyBonusAll()
        {
            _energyCostReductionNextAbility = 0;
        }

        private void ClearNextDamageBonusBySkill(Skill skill)
        {
            if (_damageIncreaseNextByAbility.ContainsKey(skill))
            {
                _damageIncreaseNextByAbility[skill] = 0;
            }

        }

        private void ClearNextEnergyBonusBySkill(Skill skill)
        {

            if (_energyCostReductionNextByAbility.ContainsKey(skill))
            {
                _energyCostReductionNextByAbility[skill] = 0;
            }
        }

        private void SpawnSkillVFX(Skill skill, GameManager gameManager)
        {
            var skillEnemyVFX = skill.EnemySkillPrefab;
            if (skillEnemyVFX != null)
            {
                var spawnedEffect = Instantiate(skillEnemyVFX, IsPlayer ? gameManager.Enemy.transform : gameManager.Player.transform);
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

        public void MakeNextMove(Character target, bool expendAllEnergy = false)
        {
            if (_skills.Count == 0 || IsAlive() == false)
            {
                return;
            }

            int index = Random.Range(0, _skills.Count);
            var skill = _skills[index];

            ActivateSkill(index, target);

            _gameManager.EndTurn(this);
        }
        public List<Skill> Skills()
        {
            return _skills;
        }
        public void EndTurn()
        {
            TriggerEnergyTick();
            TriggerAllEffectsOverTime();
        }

        private void TriggerEnergyTick()
        {
            _energy += _energyRegen;
            if (_energy > _maxEnergy)
            {
                _energy = _maxEnergy;
            }
        }
        private int AttributeBasedAddedSkillDamage(Skill skill, Character caster, Character enemy)
        {
            int addedDamage = 0;

            addedDamage += (caster.MissingHealth() * skill.DamagePercentSelfMissingHealth/100);
            addedDamage += (enemy.MissingHealth() * skill.DamagePercentEnemyMissingHealth/100);
            addedDamage += (caster.AbsorbAmount() * skill.DamagePercentSelfAbsorbAmount/100);
            addedDamage += (enemy.AbsorbAmount() * skill.DamagePercentEnemyAbsorbAmount/100);

            return addedDamage;
        }

        public int MissingHealth()
        {
            return _maxHealth - _currentHealth;
        }

        public int AbsorbAmount()
        {
            return _absorb;
        }
    }


    public class EffectOverTurns
    {
        public int Amount;
        public int Turns;
        public Skill Source;
        public EffectOverTurns(int amount, int turns, Skill source)
        {
            Amount = amount;
            Turns = turns;
            Source = source;
        }
    }
}