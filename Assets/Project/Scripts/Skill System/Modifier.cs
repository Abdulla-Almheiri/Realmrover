using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Modifier 
{
    public int BaseValue = 0;
    public int Multiplier = 0;
    public ModifierFlags Flag;

    public Modifier(int value, int multiplier, ModifierFlags flag)
    {
        BaseValue = value;
        Multiplier = multiplier;

    }

    public void Add(Modifier modifier)
    {
        this.BaseValue += modifier.BaseValue;
        this.Multiplier += modifier.Multiplier;

    }
}

public enum ModifierFlags
{
    Potency,
    Healing,

    Strength, Intellect, Agility, Faith, Power,

    CriticalChance, AutoAttackChance,
    CriticalDamage, AutoAttackDamage, AutoAttackCriticalDamage, AutoAttackCriticalChance,

    Pierce, Slash, Crush,
    Bleed,
    Fire, Frost, Dark, Holy, Spirit, Lightning, Wind,
    Burn, Poison,

    ManaCost, HealthDrain, ManaGain

}