using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName ="new_ability", menuName ="Abilities/Ability")]
public class AbilityObject: ScriptableObject
{
    public string Name;
    public int RequiredLevel;
    [TextArea(10,10)]
    public string Description = "Enter ability description here.";
    public Element Element;
    public Action Action;
    public CombatEffect CombatEffect;
    public AbilityTrigger Trigger;
}

public enum Element
{
    None,
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
    Lightning,
    Dark,
    ManaReturn,
    HealthLeech,
    ManaBurn,
    Heal,
}
