using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : ScriptableObject
{
    public static int BaseHealth = 20;
    public static int BaseMana = 5;

    public static int StrengthToMaxHealth = 5;
    public static int IntellectToMaxMana = 2;
    public static int IntellectToCriticalDamage = 1;


    public static int AgilityToAutoAttackChance = 2;

    public static int FaithToHealthRegen = 2;
    public static int FaithToManaRegen = 1;

    public static int PowerToAllDamage = 1;

    public static int CriticalDamageModifier = 100; //Double damage
    public static int BaseCriticalChance = 5;
    public static int BaseAutoAttackChance = 5;

}
