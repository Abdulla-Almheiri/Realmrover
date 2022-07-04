using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover
{
    public class LevelHoverScript : MonoBehaviour
    {
        [SerializeField]
        private int id;
        [SerializeField]
        private MainMapScript _mainMap;

        private void OnMouseEnter()
        {
            //Debug.Log("Mouse entered level");
            _mainMap.HoverOverLevel(id);
        }

        private void OnMouseExit()
        {
            //Debug.Log("Mouse exited level");
            _mainMap.HideLevelTooltip();
        }
    }
}