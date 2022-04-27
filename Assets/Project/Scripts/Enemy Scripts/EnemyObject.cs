using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new_enemy", menuName ="Characters/Enemy")]
public class EnemyObject : AgentObject
{
    public InventoryObject Loot;
    public void Act()
    {

    }

    public void DropLoot()
    {
        for(int i = 0; i<Loot.Items.Count; i++)
        {
            if (Loot.Items[i] != null)
            {
               // BattleManager.Player.Inventory.Items.Add(Loot.Items[i]);
            }
        }
    }
   
}
