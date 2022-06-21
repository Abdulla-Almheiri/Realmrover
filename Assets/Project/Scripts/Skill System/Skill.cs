using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover
{
    [CreateAssetMenu(fileName ="new skill", menuName ="Skill System/Skill")]
    public class Skill : ScriptableObject
    {
        [Header("Skill")]
        public Sprite Icon;
        public string Name = "";
        public string Description = "";
        public int Level = 1;

        [Header("Direct Effects")]
        public int EnergyCost = 0;
        public int BaseDamage = 0;
        public int Heal = 0;
        public int Absorb = 0;
        public int ReflectDamage = 0;
        public int DamagePercentMissingHealth = 0;
        public int DamagePercentAbsorbAmount = 0;

        [Header("Effects per turn")]
        public int DamagePerTurn = 0;
        public int DamageTurns = 0;
        public int ReduceDamage = 0;
        public int IncreaseDamageTaken = 0;
        public int DebuffTurns = 0;

        [Header("Enhance Next Ability")]
        public int EnhanceNextDamage = 0;
        public int EnhanceNextEnergyCost = 0;
        public Skill EnhanceNextSkill = null;

        [Header("Skill Prefab")]
        public GameObject EnemySkillPrefab = null;
        public GameObject SelfSkillPrefab = null;

        [Header("Player Animation")]
        public PlayerAnimationType AnimationType = PlayerAnimationType.SHIELD;
    }

    public enum PlayerAnimationType
    {
        SWORD,
        SHIELD
    }
}