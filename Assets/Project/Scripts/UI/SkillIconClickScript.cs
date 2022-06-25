using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover
{
    public class SkillIconClickScript : MonoBehaviour
    {
        [SerializeField]
        private SkillBarUIScript _skillBar;
        [SerializeField]
        private int _index=0;
        private void OnMouseDown()
        {
            if(_skillBar == null)
            {
                return;
            }

            _skillBar.AbilityClicked(_index);
        }
    }
}