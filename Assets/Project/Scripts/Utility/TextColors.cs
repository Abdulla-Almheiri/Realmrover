using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover
{
    [CreateAssetMenu(fileName ="new text colors", menuName ="Utility/Text Colors")]
    public class TextColors : ScriptableObject
    {
        public Color DirectDamage = Color.white;
        public Color DirectHeal = Color.green;
        public Color HealthRegen = Color.yellow;
        public Color EnergyRegen = Color.blue;
        public Color HealPerTurn = Color.cyan;
        public Color DamagePerTurn = Color.black;
        public Color CompleteAbsorb = Color.gray;
        public Color Reflect = Color.magenta;
        public Color Sacrifice = Color.red;
    }
}