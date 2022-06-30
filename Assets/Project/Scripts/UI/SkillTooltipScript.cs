using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover
{
    public class SkillTooltipScript : MonoBehaviour
    {
        [SerializeField] BattleScreenScript _battleScreen;
        public int ID;

        private void OnMouseEnter()
        {
            _battleScreen.MouseEnterSkill(ID);
        }

        private void OnMouseExit()
        {
            _battleScreen.MouseLeaveSkill(ID);
        }
    }
}