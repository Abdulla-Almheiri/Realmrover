using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName ="New Invnentory", menuName ="Items/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> Items;
    public void AddItem(ItemObject _item, int _amount)
    {
        bool hasItem = false;
        for(int i = 0 ; i< Items.Count; i++)
        {
            if (Items[i].Item == _item)
            {
                Items[i].AddAmount(_amount);
                hasItem = true;
                break;
            }
        }

        if(!hasItem)
        {
            Items.Add(new InventorySlot(_item, _amount));
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject Item;
    public int Amount;
    public InventorySlot(ItemObject _item, int _amount)
    {
        Item = _item;
        Amount = _amount;
    }

    public void AddAmount(int value)
    {
        Amount += value;
    }
}
