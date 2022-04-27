using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatEffect
{
    public Modifier Modifier;
    public CombatEffectType CombatEffectType;
    public AbilityTrigger Trigger;
    public CombatEffect(int value, int duration, int globalModifier, CombatEffectType combatEffectType)
    {
        Modifier.BaseValue = value;
        CombatEffectType = combatEffectType;
        Trigger = new AbilityTrigger(100, 0, TriggerCondition.None, TriggerType.None, Modifier.BaseValue);
    }

}

public enum CombatEffectType
{
    None,
    Strength,
    Intellect,
    Agility,
    Faith,
    Power,
    CriticalChance,
    CriticalDamage,
    AutoAttackChance,
    AutoAttackCriticalChance,
    AutoAttackCriticalDamage,
    AutoAttackDamage,
    Damage,
    ManaCost,
    Dodge,
    Absorb,
    Burn,
    Poison,
    Bleed,
    ManaGain,
    ManaDrain,
    ManaBurn,
    HealthDrain

}