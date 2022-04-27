using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName ="new_agent", menuName ="Characters/Agent")]
public class AgentObject: ScriptableObject
{
    public string Name;
    public Attributes BaseAttributes;
    private Attributes currentAttributes, maxAttributes, bonusAttributes;

    public List<AbilityObject> Abilities;
    public List<Modifier> Modifiers;
    [HideInInspector]
    public List<TriggerCondition> ActiveTriggerConditions;
    [HideInInspector]
    public List<CombatEffect> CombatEffects;


    public void Init()
    {
        ActiveTriggerConditions = new List<TriggerCondition>();

        maxAttributes = BaseAttributes.Copy();
        HandleBaseAttributes();
        currentAttributes = maxAttributes.Copy();
        bonusAttributes = new Attributes(0, 0, 0, 0, 0, 0, 0, 0);

    }

    private void HandleBaseAttributes()
    {
        bool isAtMaxHealth = false, isAtMaxMana = false;

        if (HealthPercent() == 100)
        {
            isAtMaxHealth = true;
        }

        if (ManaPercent() == 100)
        {
            isAtMaxMana = true;
        }

        maxAttributes.Health = currentAttributes.Strength * BattleManager.StrengthToMaxHealth;
        maxAttributes.Mana  = currentAttributes.Intellect * BattleManager.IntellectToMaxMana;

        if(isAtMaxHealth)
        {
            currentAttributes.Health = maxAttributes.Health;
        }

        if (isAtMaxMana)
        {
            currentAttributes.Mana = maxAttributes.Mana;
        }

        currentAttributes.CriticalChance = BattleManager.BaseCriticalChance + bonusAttributes.CriticalChance;
        currentAttributes.AutoAttackChance = BattleManager.BaseAutoAttackChance + (currentAttributes.Agility * BattleManager.AgilityToAutoAttackChance);
    }
    private void HandleBonusAttributes()
    {
        currentAttributes.Health += bonusAttributes.Health;
        currentAttributes.Mana += bonusAttributes.Mana;

        currentAttributes.Strength += bonusAttributes.Strength;
        currentAttributes.Intellect += bonusAttributes.Intellect;
        currentAttributes.Agility += bonusAttributes.Agility;
        currentAttributes.Faith += bonusAttributes.Faith;
        currentAttributes.Power += bonusAttributes.Power;

        currentAttributes.CriticalChance += bonusAttributes.CriticalChance;
        currentAttributes.AutoAttackChance += bonusAttributes.AutoAttackChance;
        currentAttributes.CriticalDamage += bonusAttributes.CriticalDamage;

    }

    public void Turn()
    {

    }

    public int HealthPercent()
    {
        return 100 * currentAttributes.Health / maxAttributes.Health;
    }
    public int ManaPercent()
    {
        return 100 * currentAttributes.Mana / maxAttributes.Mana;
    }

    private void BoundHealthMana()
    {
        if(currentAttributes.Health > maxAttributes.Health)
        {
            currentAttributes.Health = maxAttributes.Health;
        }

        if (currentAttributes.Mana > maxAttributes.Mana)
        {
            currentAttributes.Mana= maxAttributes.Mana;
        }
    }

    private void HandleAttributesTriggerConditions()
    {
        int healthPercent = HealthPercent();
        int manaPercent = ManaPercent();

        if (healthPercent >= 80)
        {
            ActiveTriggerConditions.Add(TriggerCondition.HealthHigh);
        } else
        if(healthPercent < 20)
        {
            ActiveTriggerConditions.Add(TriggerCondition.HealthLow);
        } else
        {
            ActiveTriggerConditions.Remove(TriggerCondition.HealthLow);
            ActiveTriggerConditions.Remove(TriggerCondition.HealthHigh);
        }

        if (manaPercent >= 80)
        {
            ActiveTriggerConditions.Add(TriggerCondition.ManaHigh);
        }
        else
        if (manaPercent < 20)
        {
            ActiveTriggerConditions.Add(TriggerCondition.ManaLow);
        }
        else
        {
            ActiveTriggerConditions.Remove(TriggerCondition.ManaLow);
            ActiveTriggerConditions.Remove(TriggerCondition.ManaHigh);
        }
    }

    private void HandleCombatEffectsTriggers()
    {
        if(FindFirstCombatEffectByType(CombatEffectType.Bleed) != null)
        {
            ActiveTriggerConditions.Add(TriggerCondition.Bleed);
        } else
        {
            ActiveTriggerConditions.Remove(TriggerCondition.Bleed);
        }

        if (FindFirstCombatEffectByType(CombatEffectType.Poison) != null)
        {
            ActiveTriggerConditions.Add(TriggerCondition.Poison);
        }
        else
        {
            ActiveTriggerConditions.Remove(TriggerCondition.Poison);
        }


        if (FindFirstCombatEffectByType(CombatEffectType.Burn) != null)
        {
            ActiveTriggerConditions.Add(TriggerCondition.Burn);
        }
        else
        {
            ActiveTriggerConditions.Remove(TriggerCondition.Burn);
        }
    }

    private void DoAction(Action action, AgentObject target)
    {
        if(action.ManaCost > currentAttributes.Mana)
        {
            BattleMessage(BattleMessageType.ManaTooLow);
            return;
        }

        int result = CalculateActionRaw(action);
        int value = 0;
        int multiplier = 0;

        //If none damaging or healing, then return
        if (result == 0 && action.ManaDrainPercent == 0 && action.ManaReturn == 0)
        {
            return;
        }

        //Healing
        Modifier healingModifier = FindFirstModifier(ModifierFlags.Healing);

        if (result < 0)
        {
            result -= healingModifier.BaseValue;
            result *= (healingModifier.Multiplier / 100);
            target.TakeDamage(result);
        } else
        {

        }


        //Check Potency modifier on action
        Modifier potencyModifier = FindFirstModifierInAction(action, ModifierFlags.Potency);
        if(potencyModifier != null)
        {
            result += potencyModifier.BaseValue;
            result *= (potencyModifier.Multiplier / 100);
        }

        //Critical Chance Calculation
        bool isCritical = false;
        int criticalChance = currentAttributes.CriticalChance;
        Modifier criticalModifier = FindFirstModifierInAction(action, ModifierFlags.CriticalChance);

        if (criticalModifier != null)
        {
            criticalChance += criticalModifier.BaseValue;
        }


        //Critical Damage Calculation
        int criticalDamage = currentAttributes.CriticalDamage;
        Modifier criticalDamageModifier = FindFirstModifierInAction(action, ModifierFlags.CriticalDamage);

        if (criticalDamageModifier != null)
        {
            criticalDamage += criticalDamageModifier.BaseValue;
        }

        // Critical Chance Roll
        if (Random.Range(0, 100) < criticalChance)
        {
            isCritical = true;
            result *= (100 + criticalDamage);
            result /= 100;
        }





        // Resource Gains
        if (action.ManaReturn > 0)
        {
            GainMana(action.ManaReturn);
        }

        if (action.ManaDrainPercent > 0)
        {
            GainMana(result * action.ManaDrainPercent / 100);
        }
        //Auto Attack Check
        Modifier autoAttackChanceModifier = FindFirstModifierInAction(action, ModifierFlags.AutoAttackChance);
        Modifier autoAttackDamageModifier = FindFirstModifierInAction(action, ModifierFlags.AutoAttackDamage);
        Modifier autoAttackCriticalChanceModifier = FindFirstModifierInAction(action, ModifierFlags.AutoAttackCriticalChance);
        Modifier autoAttackCriticalDamageModifier = FindFirstModifierInAction(action, ModifierFlags.AutoAttackCriticalDamage);

        bool hasTriggeredAutoAttack = false;
        int autoAttackChance = currentAttributes.AutoAttackChance;
        autoAttackChance += autoAttackChanceModifier.BaseValue;

        if (Random.Range(0, 100) < autoAttackChance)
        {
            hasTriggeredAutoAttack = true;
            AutoAttack(new Modifier(10,10, ModifierFlags.Strength));
        }


    }

    private void AutoAttack(Modifier modifier)
    {

    }
    private Modifier GetModifiersFromAction(Action action)
    {
        return null;
    }
    private void AddModifier(Modifier modifier)
    {
        Modifier tempModifier = FindFirstModifier(modifier.Flag);
        if (tempModifier == null)
        {
            Modifiers.Add(modifier);
        } else
        {
            tempModifier.Add(modifier);

        }
    }

    private Modifier FindFirstModifier(ModifierFlags flag)
    {

        for (int i = 0; i < Modifiers.Count; i++)
        {
            if (Modifiers[i].Flag == flag)
            {
                return Modifiers[i];
            }
        }
        return null;

    }
    private List<Modifier> FindAllModifiers(ModifierFlags flag)
    {
        List<Modifier> modifiers = new List<Modifier>();

        for (int i = 0; i < Modifiers.Count; i++)
        {
            if (Modifiers[i].Flag == flag)
            {
                modifiers.Add(Modifiers[i]);
            }
        }
        return modifiers;

    }

    private Modifier FindFirstModifierInAction(Action action, ModifierFlags flag)
    {
        for(int i =0; i<action.InternalModifiers.Count; i++)
        {
            if(action.InternalModifiers[i].Flag == flag)
            {
                return action.InternalModifiers[i];
            }
        }
        return null;
    }
    private void TakeDamage(int value)
    {
        currentAttributes.Health -= value;
        BoundHealthMana();
    }
    private void GainMana(int value)
    {
        currentAttributes.Mana += value;
        BoundHealthMana();
    }
    private int CalculateActionRaw(Action action)
    {
        int result = 0;
        result += action.Coefficient.Level * currentAttributes.Level/100;
        result += action.Coefficient.Strength * currentAttributes.Strength/100;
        result += action.Coefficient.Intellect * currentAttributes.Intellect/100;
        result += action.Coefficient.Agility * currentAttributes.Agility/100;
        result += action.Coefficient.Faith * currentAttributes.Faith/100;
        result += action.Coefficient.Power * currentAttributes.Power/100;
        return result;
    }
    public void BattleMessage(BattleMessageType messageType)
    {
        
    }
    private CombatEffect FindFirstCombatEffectByType(CombatEffectType type)
    {
        for(int i = 0; i< CombatEffects.Count; i++)
        {
            if(CombatEffects[i].CombatEffectType == type)
            {
                return CombatEffects[i];
            }
        }
        return null;
    }
}

public enum BattleMessageType
{
    ManaTooLow,
    Stunned,

}