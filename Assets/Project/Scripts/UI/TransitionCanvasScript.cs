using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Realmrover
{
    public class TransitionCanvasScript : MonoBehaviour
    {
        [SerializeField] private Image _panel;

        public Image ScreenPanel()
        {
            return _panel;
        }
    }
}