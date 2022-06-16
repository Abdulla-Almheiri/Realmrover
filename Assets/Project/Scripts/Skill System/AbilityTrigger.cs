using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[System.Serializable]
public class AbilityTrigger
{
    [Range(0,100)]
    public int TriggerChance = 100;
    public int TriggerDelay = 0;
    public TriggerCondition TriggerCondition;
    public TriggerType Trigger;
    public int AttributeValue = 0;

    public AbilityTrigger(int chance, int delay, TriggerCondition condition, TriggerType type, int attributeValue)
    {
        TriggerChance = chance;
        TriggerDelay = delay;
        TriggerCondition = condition;
        Trigger = type;
        AttributeValue = attributeValue;

    }
}
public enum TriggerType
{
    None,
    AnyAction,
    AnyDamage,
    AnyHeal,
    Pierce,
    Slash,
    Bleed,
    Poison,
    Crush,
    Frost,
    Fire,
    Burn,
    Holy,
    Spirit,
    Dark,
    Physical,
    Magic,
    Melee,
    AutoAttack,
    Critical,
    AutoAttackCritical,
    DamageOverTime,
    Enhancement,
    Hinderance,
    Stun
}

public enum TriggerCondition
{
    None,
    FirstTurn,
    Always,

    HealthLow,
    HealthHigh,
    HealthFull,
    ManaLow,
    ManaHigh,
    ManaFull,

    Burn,
    Poison,
    Bleed,

    Stun,
    
    AutoAttack,
    AutoAttackCritical,
    Critical
}