using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new item", menuName ="Items/Item")]
public class ItemObject : ScriptableObject
{
    public int ID;
    public string Name;
    [TextArea(15,20)]
    public string Description;

    public int RequiredLevel;

    public bool Stackable;
    public int SellAmount;

}
