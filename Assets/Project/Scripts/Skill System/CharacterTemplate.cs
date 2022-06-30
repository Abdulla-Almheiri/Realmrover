using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover
{
    [CreateAssetMenu(fileName ="new character template", menuName ="Characters/Character")]
    public class CharacterTemplate : ScriptableObject
    {
        public string Name = "";
        [TextArea]
        public string Description = "";
        public int Level;
        public int Health = 0;
        public int HealthRegen = 0;
        public int HealthRegenPerLevel = 0;
        public int MaxHealthPerLevel = 0;
        public int Energy = 0;
        public int MaxEnergyPerLevel = 0;
        public int EnergyRegen = 0;
        public int EnergyRegenPerLevel = 0;

        public Sprite PortraitImage;
        public RuntimeAnimatorController AnimationData;
        public List<Skill> Skills;

    }
}