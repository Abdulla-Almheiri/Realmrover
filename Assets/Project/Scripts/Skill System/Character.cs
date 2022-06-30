using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover
{
    public class Character : MonoBehaviour
    {
        private int _level = 1;
        private bool _alive = false;
        public bool IsPlayer = false;
        private GameManager _gameManager;
        private CharacterTemplate _template;

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
        private Dictionary<Skill, EffectOverTurns> _increaseDamageForTurnsEffects = new Dictionary<Skill, EffectOverTurns>();
        private Dictionary<Skill, EffectOverTurns> _reduceDamageTakenForTurnsEffects = new Dictionary<Skill, EffectOverTurns>();
        private Dictionary<Skill, EffectOverTurns> _reduceEnergyCostForTurnsEffects = new Dictionary<Skill, EffectOverTurns>();

        private Dictionary<Skill, EffectOverTurns> _damageOverTimeEffects = new Dictionary<Skill, EffectOverTurns>();
        private Dictionary<Skill, EffectOverTurns> _damageTakenIncreaseForTurnsEffects = new Dictionary<Skill, EffectOverTurns>();

        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private void OnMouseEnter()
        {
            _gameManager.ShowTooltip(this);
        }

        private void OnMouseExit()
        {
            _gameManager.HideTooltip();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.T))
            {
                FadeOut(2f);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                FadeIn(2f);
            }
        }
        public void Initialize(GameManager gameManager)
        {
            if (gameManager == null)
            {
                return;
            }
            _gameManager = gameManager;

            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
        }

        public void FadeOut(float duration)
        {
            if(duration <= 0f)
            {
                _spriteRenderer.color = new Color(1, 1, 1, 0);
                return;
            }

            StartCoroutine(FadeOutCO(duration));
        }

        private IEnumerator FadeOutCO(float duration)
        {
            float remainingDuration = duration;
            while(remainingDuration >=0f)
            {
                _spriteRenderer.color = new Color(1, 1, 1, remainingDuration / duration);
                remainingDuration -= Time.deltaTime;
                yield return null;
            }
        }

        public void FadeIn(float duration)
        {
            if (duration <= 0f)
            {
                _spriteRenderer.color = new Color(1, 1, 1, 1);
                return;
            }

            StartCoroutine(FadeInCO(duration));
        }
        private IEnumerator FadeInCO(float duration)
        {
            float remainingDuration = duration;
            while (remainingDuration >= 0f)
            {
                _spriteRenderer.color = new Color(1, 1, 1, 1- ( remainingDuration / duration));
                remainingDuration -= Time.deltaTime;
                yield return null;
            }
        }

        public void SetCharacterTemplate(CharacterTemplate template, int level = 1)
        {
            if(template == null)
            {
                Debug.LogWarning("Setting Character Template: Template is null!");
            }
            _template = template;
            _level = level;
            _alive = true;

            _maxHealthPerLevel = template.MaxHealthPerLevel;
            _maxEnergyPerLevel = template.MaxHealthPerLevel;
            _EnergyRegenPerLevel = template.EnergyRegenPerLevel;
            _HealthRegenPerLevel = template.HealthRegenPerLevel;

            //_level = template.Level;
            _currentHealth = template.Health + (_level - 1) * _maxHealthPerLevel;
            _maxHealth = _currentHealth;
            _healthRegen = template.HealthRegen + (_level - 1) * _HealthRegenPerLevel;
            _energy = template.Energy + (_level - 1) * _maxEnergyPerLevel;
            _maxEnergy = _energy;
            _energyRegen = template.EnergyRegen + (_level - 1) * _EnergyRegenPerLevel;

            _animator.runtimeAnimatorController = template.AnimationData;
            _skills = new List<Skill>(template.Skills);
            _animator.SetTrigger("Revive");
        }

        public bool ActivateSkill(int index, Character target)
        {
            if (IsAlive() == false)
            {
                return false;
            }

            if(index >= _skills.Count)
            {
                return false;
            }

            var skill = _skills[index];

            if (skill == null)
            {
                return false;
            }

            if(target == null)
            {
                return false;
            }

            if(skill.SacrificeDamage > 0)
            {
                if(skill.SacrificeDamage >= _currentHealth && skill.SacrificeDamageLethal == false)
                {
                    return false;
                } else
                {
                    TakeDamage(skill.SacrificeDamage, FloatingTextType.SACRIFICE, this, skill, true, false, true);
                }
            }

            if (HasEnergyFor(skill, out int energyCost) == false)
            {
                return false;
            }


            _energy -= energyCost;
            Debug.Log("Energy used :            " + energyCost);

            ClearNextEnergyBonusAll();
            ClearNextEnergyBonusBySkill(skill);



            if (skill.ReduceEnergyCostForTurns > 0)
            {
                ApplyReduceEnergyCostForTurnsEffect(skill);
            }


            if (skill.BaseDamage > 0)
            {
                target.TakeDamage(CalculateDirectDamageNextAbility(skill, this, target), FloatingTextType.DIRECT_DAMAGE,this, skill, false, true, false);
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
                target.ApplyDamageOverTimeEffect(skill);
            }


            if (skill.Heal > 0)
            {
                Heal(skill.Heal, FloatingTextType.DIRECT_HEAL);
            }

            if(skill.HealPerTurn > 0)
            {
                ApplyHealOverTimeEffect(skill);
            }

            if(skill.IncreaseDamageForTurns > 0)
            {
                ApplyDamageIncreaseForTurnsEffect(skill);
            }

            if(skill.ReduceDamageTakenForTurns > 0)
            {
                ApplyReduceDamageTakenForTurnsEffect(skill);
            }

            if(skill.EnhanceNextEnergyCost > 0)
            {
                ApplyNextEnergyReductionBonusFromSkill(skill);
            }

            if(skill.EnhanceNextDamage > 0)
            {
                ApplyNextDamageBonusFromSkill(skill);
            }

            SpawnSkillVFX(skill, _gameManager);

            _gameManager.UpdateResourcesUI();
            return true;
        }

        public void ApplyEffectsOverTime(Skill skill, GameManager gameManager)
        {

        }

        public void ApplyDamageOverTimeEffect(Skill skill)
        {
            if (_damageOverTimeEffects.ContainsKey(skill))
            {
                if (skill.DamageOverTimeStacks == false)
                {
                    if (skill.DamageOverTimeRefreshesDuration == true)
                    {
                        _damageOverTimeEffects[skill].Turns = skill.DamageTurns;
                    }

                    return;
                }
                else
                {
                    _damageOverTimeEffects[skill].Amount += skill.DamagePerTurn;

                }
            }
            else
            {
                _damageOverTimeEffects.Add(skill, new EffectOverTurns(skill.DamagePerTurn, skill.DamageTurns, skill));
            }
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
        private void ApplyDamageIncreaseForTurnsEffect(Skill skill)
        {
            if(_increaseDamageForTurnsEffects.ContainsKey(skill) == true)
            {
                _increaseDamageForTurnsEffects[skill].Turns = skill.IncreaseDamageTurns;

            } else
            {
                _increaseDamageForTurnsEffects.Add(skill, new EffectOverTurns(skill.IncreaseDamageForTurns, skill.IncreaseDamageTurns, skill));

            }
        }

        private void ApplyReduceDamageTakenForTurnsEffect(Skill skill)
        {
            if (_reduceDamageTakenForTurnsEffects.ContainsKey(skill) == true)
            {
                _reduceDamageTakenForTurnsEffects[skill].Turns = skill.ReduceDamageTakenTurns;

            }
            else
            {
                _reduceDamageTakenForTurnsEffects.Add(skill, new EffectOverTurns(skill.ReduceDamageTakenForTurns, skill.ReduceDamageTakenTurns, skill));

            }
        }
        private void ApplyDamageTakenIncreaseForTurnsEffect(Skill skill)
        {
            if (_damageTakenIncreaseForTurnsEffects.ContainsKey(skill) == true)
            {
                _damageTakenIncreaseForTurnsEffects[skill].Turns = skill.IncreaseDamageTakenTurns;
            }
            else
            {
                _damageTakenIncreaseForTurnsEffects.Add(skill, new EffectOverTurns(skill.IncreaseDamageTakenForTurns, skill.IncreaseDamageTakenTurns, skill));
            }
        }
        private void ApplyReduceEnergyCostForTurnsEffect(Skill skill)
        {
            if (_reduceEnergyCostForTurnsEffects.ContainsKey(skill) == true)
            {
                _reduceEnergyCostForTurnsEffects[skill].Turns = skill.ReduceEnergyCostTurns;
            }
            else
            {
                _reduceEnergyCostForTurnsEffects.Add(skill, new EffectOverTurns(skill.ReduceEnergyCostForTurns, skill.ReduceEnergyCostTurns, skill));
            }
        }

        private void TriggerAllEffectsOverTime()
        {
            if (IsAlive() == false)
            {
                return;
            }

            TriggerReduceEnergyCostForTurnsEffects();
            TriggerIncreaseDamageTakenForTurnsEffects();
            TriggerReduceDamageTakenEffects();
            TriggerIncreaseDamageDoneEffects();
            TriggerHealOverTimeEffects();
            TriggerDamageOverTimeEffects();
        }

        private void TriggerHealOverTimeEffects()
        {
            if (IsAlive() == false)
            {
                return;
            }

            if (_healOverTimeEffects.Count == 0)
            {
                return;
            }
            var values = new List<EffectOverTurns>(_healOverTimeEffects.Values);
            foreach (EffectOverTurns effect in values)
            {
                if(effect.Turns == 0 )
                {
                    _healOverTimeEffects.Remove(effect.Source);
                    continue;
                }
                Heal(effect.Amount, FloatingTextType.HEAL_PER_TURN);
                effect.Turns--;
            }

        }
        private void TriggerDamageOverTimeEffects()
        {
            if (IsAlive() == false)
            {
                return;
            }

            if (_damageOverTimeEffects.Count == 0)
            {
                return;
            }
            var values = new List<EffectOverTurns>(_damageOverTimeEffects.Values);
            foreach (EffectOverTurns effect in values)
            {
                if (effect.Turns == 0)
                {
                    _damageOverTimeEffects.Remove(effect.Source);
                    continue;
                }
                var attacker = _gameManager.CurrentPlayerCharacter() == this ? _gameManager.CurrentEnemyCharacter() : _gameManager.CurrentPlayerCharacter();
                TakeDamage(effect.Amount, FloatingTextType.DAMAGE_PER_TURN,attacker, effect.Source, true, false, false) ;
                effect.Turns--;
            }
        }

        private void TriggerIncreaseDamageDoneEffects()
        {
            var values = new List<EffectOverTurns>(_increaseDamageForTurnsEffects.Values);
            foreach (EffectOverTurns effect in values)
            {
                if (effect.Turns <= 0)
                {
                    _increaseDamageForTurnsEffects.Remove(effect.Source);
                }
                else
                {
                    effect.Turns--;
                }
            }
        }

        private void TriggerReduceDamageTakenEffects()
        {
            var values = new List<EffectOverTurns>(_reduceDamageTakenForTurnsEffects.Values);
            foreach (EffectOverTurns effect in values)
            {
                if (effect.Turns <= 0)
                {
                    _reduceDamageTakenForTurnsEffects.Remove(effect.Source);
                }
                else
                {
                    effect.Turns--;
                }
            }
        }
        private void TriggerIncreaseDamageTakenForTurnsEffects()
        {
            var values = new List<EffectOverTurns>(_damageTakenIncreaseForTurnsEffects.Values);
            foreach (EffectOverTurns effect in values)
            {
                if (effect.Turns <= 0)
                {
                    _damageTakenIncreaseForTurnsEffects.Remove(effect.Source);
                }
                else
                {
                    effect.Turns--;
                }
            }
        }
        private void TriggerReduceEnergyCostForTurnsEffects()
        {
            var values = new List<EffectOverTurns>(_reduceEnergyCostForTurnsEffects.Values);
            foreach (EffectOverTurns effect in values)
            {
                if (effect.Turns <= 0)
                {
                    _reduceEnergyCostForTurnsEffects.Remove(effect.Source);
                }
                else
                {
                    effect.Turns--;
                }
            }
        }
        public void TakeDamage(int amount, FloatingTextType type, Character attacker, Skill skill, bool ignoreReflect = false, bool isDirect = true, bool ignoreAbsorb=false)
        {
            if (IsAlive() == false)
            {
                return;
            }

            if (skill.IncreaseDamageTakenForTurns > 0 && isDirect == true)
            {
                ApplyDamageTakenIncreaseForTurnsEffect(skill);
            }

            if (ignoreAbsorb == false)
            {
                amount = (amount * CalculateMultiplierReduceDamageTakenForTurnsEffects()) / 100;

                //calculate DamageTakenIncreaseForTurnsEffect here
                amount = amount * ((100 + CalculateMultiplierIncreaseDamageTakenForTurnsEffects()) / 100);
            }

            if (_reflect > 0 && ignoreReflect == false)
            {
                
                if (_reflect >= amount)
                {
                    attacker.TakeDamage(amount, FloatingTextType.REFLECT, this, skill, true, false, true);
                    _reflect -= amount;
                } else
                {
                    attacker.TakeDamage(_reflect, FloatingTextType.REFLECT, this, skill,  true, false, true);
                    _reflect = 0;
                }

            }

            if (_absorb > 0 && ignoreAbsorb == false)
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

            bool spawnAtEnemy = this == _gameManager.CurrentEnemyCharacter() ? true : false;
            if (type == FloatingTextType.SACRIFICE)
            {
                spawnAtEnemy = !spawnAtEnemy;
            }
            
            _gameManager.QueueCombatText(amount, type, spawnAtEnemy);
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
                _gameManager.ReportCharacterDeath(this);
            }
        }

        public void Heal(int amount, FloatingTextType textType, bool showFloatingText = true)
        {
            if (showFloatingText == true)
            {
                var atEnemy = _gameManager.CurrentPlayerCharacter() == this ? false :true;
                _gameManager.QueueCombatText(amount, textType , atEnemy);
            }
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
       
            finalCost *= 10000;
            finalCost = finalCost * ((100 - _energyCostReductionNextAbility) )/ 100;
            finalCost = finalCost * ((100 - costReductionNextByAbility)) / 100;
            finalCost = finalCost * ((100 - CalculateMultiplierEnergyCostReductionForTurnsEffects())) / 100;
            finalCost /= 10000;

            /*finalCost = finalCost - ((100 - _energyCostReductionNextAbility) * finalCost) / 100;
            finalCost = finalCost - ((100 - costReductionNextByAbility) * finalCost) / 100;
            finalCost = finalCost - ((100 - CalculateMultiplierEnergyCostReductionForTurnsEffects()) * finalCost)/ 100;
            */

            finalEnergyCost = finalCost;

            if (_energy >= finalCost)
            {
                return true;
            } else
            {
                return false;
            }

        }

        private int CalculateDirectDamageNextAbility(Skill skill, Character caster, Character receiver)
        {
            int result = skill.BaseDamage;
            result += AttributeBasedAddedSkillDamage(skill, caster, receiver);
            result += _damageIncreaseNextAbility * skill.BaseDamage / 100;

            if (_damageIncreaseNextByAbility.ContainsKey(skill))
            {
                result += (_damageIncreaseNextByAbility[skill] * skill.BaseDamage) / 100;

            }


           result += skill.BaseDamage * CalculateMultiplierDamageIncreaseForTurnsEffects() /100;

            return result;
        }
        private int CalculateMultiplierReduceDamageTakenForTurnsEffects()
        {
            int result = 100;
            foreach(EffectOverTurns effect in _reduceDamageTakenForTurnsEffects.Values)
            {
                result = result * (100-effect.Amount)/100;
            }

            return result;
        }
        private int CalculateMultiplierIncreaseDamageTakenForTurnsEffects()
        {
            int result = 0;
            foreach (EffectOverTurns effect in _damageTakenIncreaseForTurnsEffects.Values)
            {
                result += effect.Amount;
            }

            Debug.Log("Multiplier calculated to be : " + result);
            return result;
        }

        private int CalculateMultiplierDamageIncreaseForTurnsEffects()
        {
            int result = 0;
            if(_increaseDamageForTurnsEffects.Count == 0)
            {
                return 0;
            }

            foreach(EffectOverTurns effect in _increaseDamageForTurnsEffects.Values)
            {
                result += effect.Amount;
            }

            
            return result;
        }
        private int CalculateMultiplierEnergyCostReductionForTurnsEffects()
        {
            int result = 100;
            if (_reduceEnergyCostForTurnsEffects.Count == 0)
            {
                return 0;
            }

            foreach (EffectOverTurns effect in _reduceEnergyCostForTurnsEffects.Values)
            {
                result =  result * (100-effect.Amount)/100;
            }


            return result;
        }
        private void ApplyNextEnergyReductionBonusFromSkill(Skill skill)
        {
            if (skill == null)
            {
                return;
            }

            if (skill.EnhanceNextSkill == null)
            {
                _energyCostReductionNextAbility = skill.EnhanceNextEnergyCost;
            } else
            {

                if (_energyCostReductionNextByAbility.ContainsKey(skill.EnhanceNextSkill))
                {
                    _energyCostReductionNextByAbility[skill.EnhanceNextSkill] += skill.EnhanceNextEnergyCost;
                }
                else
                {
                    _energyCostReductionNextByAbility.Add(skill.EnhanceNextSkill, skill.EnhanceNextEnergyCost);
                }
            }
        }
        private void ApplyNextDamageBonusFromSkill(Skill skill)
        {
            if (skill == null)
            {
                return;
            }

            if (skill.EnhanceNextSkill == null)
            {
                _damageIncreaseNextAbility += skill.EnhanceNextDamage;
            }
            else
            {
                if (_damageIncreaseNextByAbility.ContainsKey(skill.EnhanceNextSkill))
                {
                    _damageIncreaseNextByAbility[skill.EnhanceNextSkill] += skill.EnhanceNextDamage;
                }
                else
                {
                    _damageIncreaseNextByAbility.Add(skill.EnhanceNextSkill, skill.EnhanceNextDamage);
                    Debug.Log(skill.Name + "  has just added +" + skill.EnhanceNextDamage + "% damage to " + skill.EnhanceNextSkill.Name);
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
        private bool ClearHealOverTimeEffects(Skill skill = null)
        {
            if (skill != null)
            {
                return _healOverTimeEffects.Remove(skill);
            } else
            {
                bool status = _healOverTimeEffects.Count > 0;
                _healOverTimeEffects.Clear();
                Debug.Log("HEAL OVER TIME CLEARED. SKILL NULL.");
                return status;
            }
        }

        private bool ClearDamageOverTimeEffects(Skill skill = null)
        {
            if (skill != null)
            {
                return _damageOverTimeEffects.Remove(skill);
            }
            else
            {
                bool status = _damageOverTimeEffects.Count > 0;
                _damageOverTimeEffects.Clear();
                return status;
            }
        }

        private void ClearAllEffects()
        {
            ClearNextDamageBonusAll();
            ClearNextEnergyBonusAll();
            ClearHealOverTimeEffects();
            ClearDamageOverTimeEffects();
            ClearNextDamageBonusAll();
            ClearNextEnergyBonusAll();
            _increaseDamageForTurnsEffects.Clear();
            _reduceDamageTakenForTurnsEffects.Clear();
            _reduceEnergyCostForTurnsEffects.Clear();
            _damageTakenIncreaseForTurnsEffects.Clear();
            _absorb = 0;
            _reflect = 0;
        }

        private void SpawnSkillVFX(Skill skill, GameManager gameManager)
        {
            var skillEnemyVFX = skill.EnemySkillPrefab;

            var isPlayer = this == _gameManager.CurrentPlayerCharacter();
            if (skillEnemyVFX != null)
            {
                var spawnedEffect = Instantiate(skillEnemyVFX, isPlayer ? gameManager.EnemySpawnPoint : gameManager.PlayerSpawnPoint);
                spawnedEffect.transform.parent = null;

            }

            var skillSelfVFX = skill.SelfSkillPrefab;
            if (skillSelfVFX != null)
            {
                var spawnedEffect = Instantiate(skillSelfVFX, transform);
                spawnedEffect.transform.parent = null;

            }

            if (isPlayer)
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

        public bool MakeNextMove(Character target, bool expendAllEnergy = false)
        {
            if (_skills.Count == 0 || IsAlive() == false)
            {
                return false;
            }

            int index = Random.Range(0, _skills.Count);
            var skill = _skills[index];

            if(ActivateSkill(index, target) == false)
            {
                return false;
            }

            _gameManager.EndTurn(this);
            return true;
        }
        public List<Skill> Skills()
        {
            return _skills;
        }
        public void EndTurn()
        {
            TriggerHealthRegenTick();
            TriggerEnergyTick();
            TriggerAllEffectsOverTime();
        }
        public void EndBattle()
        {
            ClearAllEffects();
             _currentHealth = MaxHealth();
             _energy = MaxEnergy();

        }
        private void TriggerEnergyTick()
        {
            if(IsAlive() == false)
            {
                return;
            }

            _energy += _energyRegen;
            if (_energy > _maxEnergy)
            {
                _energy = _maxEnergy;
            }
        }

        private void TriggerHealthRegenTick()
        {
            if (IsAlive() == false)
            {
                return;
            }

            Heal(_healthRegen, FloatingTextType.HEALTH_REGEN, false);
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

        public int MaxHealth()
        {
            return _maxHealth;
        }

        public int CurrentHealth()
        {
            return _currentHealth;
        }

        public int MaxEnergy()
        {
            return _maxEnergy;
        }

        public int CurrentEnergy()
        {
            return _energy;
        }

        public CharacterTemplate Template()
        {
            return _template;
        }
    }

    public enum DamageType
    {
        DIRECT,
        DAMAGE_OVER_TURNS,
        REFLECT,
        SACRIFICE
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