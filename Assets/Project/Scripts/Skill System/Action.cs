using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Action 
{
    public int ManaCost = 0;
    public Coefficient Coefficient;
    public List<Modifier> InternalModifiers;
    public ModifierFlags Element;
    [Space(15)]
    [Header("Resrouce Drain/Gain")]
    public int HealthDrainPercent = 0;
    public int ManaDrainPercent = 0;
    public int ManaReturn = 0;



}

