using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover
{
    [CreateAssetMenu(fileName ="new tutorial", menuName ="Tutorial/New Tutorial")]
    public class TutorialData : ScriptableObject
    {
        public List<TutorialStep> TutorialSteps;

    }

    [System.Serializable]
    public class TutorialStep
    {
        [TextArea]
        public string Message;
        public Skill WaitForSkill;
        public bool WaitForEndOfTurn = false;

        public TutorialStep(string message, Skill waitForSkill, bool waitForEndOfTurn)
        {
            Message = message;
            WaitForSkill = waitForSkill;
            WaitForEndOfTurn = waitForEndOfTurn;
        }
    }
}