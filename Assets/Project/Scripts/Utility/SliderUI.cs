using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Realmrover
{
    public class SliderUI : MonoBehaviour
    {
        private Slider _slider;
        void Start()
        {
            _slider = GetComponent<Slider>();
        }

        public void UpdateValue(float value)
        {
            _slider.value = Mathf.Clamp(value, 0.0f, 1.0f);
        }
    }
}