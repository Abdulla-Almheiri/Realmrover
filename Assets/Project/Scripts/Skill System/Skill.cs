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
        [TextArea]
        public string Description = "";
        public int Level = 1;

        [Header("Direct Effects")]
        public int EnergyCost = 0;
        public int EnergyGain = 0;
        public int BaseDamage = 0;
        public int SacrificeDamage = 0;
        public bool SacrificeDamageLethal = false;
        public int Heal = 0;
        public int Absorb = 0;
        public int ReflectDamage = 0;
        public int DamagePercentSelfMissingHealth = 0;
        public int DamagePercentEnemyMissingHealth = 0;
        public int DamagePercentSelfAbsorbAmount = 0;
        public int DamagePercentEnemyAbsorbAmount = 0;

        [Header("Buffs per turn")]
        public int HealPerTurn = 0;
        public int HealTurns = 0;
        public bool HealOverTimeStacks = false;
        public bool HealOverTimeRefreshesDuration = true;

        public int IncreaseDamageForTurns = 0;
        public int IncreaseDamageTurns = 0;

        public int ReduceDamageTakenForTurns = 0;
        public int ReduceDamageTakenTurns = 0;

        public int ReduceEnergyCostForTurns = 0;
        public int ReduceEnergyCostTurns = 0;

        [Header("Debuffs per turn")]
        public int DamagePerTurn = 0;
        public int DamageTurns = 0;
        public bool DamageOverTimeStacks = false;
        public bool DamageOverTimeRefreshesDuration = true;

        public int ReduceDamageDone = 0;
        public int ReduceDamageDoneTurns = 0;

        public int IncreaseDamageTakenForTurns = 0;
        public int IncreaseDamageTakenTurns = 0;

        [Header("Enhance Next Ability")]
        public int EnhanceNextDamage = 0;
        public int EnhanceNextEnergyCost = 0;
        public Skill EnhanceNextSkill = null;

        [Header("Skill Prefab")]
        public GameObject EnemySkillPrefab = null;
        public GameObject SelfSkillPrefab = null;

        [Header("Skill SFX")]
        public AudioClip SoundEffect = null;

        [Header("Camera Shake")]
        [Range(0, 3)]
        public int CameraShakePower = 0;

        [Header("Player Animation")]
        public PlayerAnimationType AnimationType = PlayerAnimationType.SHIELD;
    }

    public enum PlayerAnimationType
    {
        SWORD,
        SHIELD
    }
}