using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Realmrover.SkillSystem
{
    [CreateAssetMenu(fileName ="new skill", menuName ="Skill System/Skill")]
    public class Skill : ScriptableObject
    {
        [SerializeField] private string _name;

    }
}