using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attributes
{
    public int Level=0;
    public int Health=0;
    public int Mana=0;
    public int Strength=0;
    public int Intellect=0;
    public int Agility=0;
    public int Faith=0;
    public int Power=0;
    public int CriticalChance=0;
    public int AutoAttackChance=0;
    public int CriticalDamage = BattleManager.CriticalDamageModifier;

    public  Attributes(int level, int maxHealth, int maxMana, int strength, int intellect, int agility, int faith, int power)
    {
        Level = level;
        Health = maxHealth;
        Mana = maxMana;
        Strength = strength;
        Intellect = intellect;
        Agility = agility;
        Faith = faith;
        Power = power;
    }

    public Attributes Copy()
    {
        return new Attributes(this.Level, this.Health, this.Mana, this.Strength, this.Intellect, this.Agility, this.Faith, this.Power);
    }
}
