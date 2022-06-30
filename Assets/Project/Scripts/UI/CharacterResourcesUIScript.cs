using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Realmrover
{
    public class CharacterResourcesUIScript : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _characterHealthBar;
        private float _characterHealthBarOriginalWidth;
        [SerializeField]
        private TMP_Text _healthText;

        [SerializeField]
        private RectTransform _characterEnergyBar;
        private float _characterEnergyBarOriginalWidth;

        [SerializeField]
        private TMP_Text _energyText;


        [SerializeField]
        private RectTransform _characterAbsorbBar;
        private float _characterAbsorbBarOriginalWidth;
        private int _characterAbsorbAmount = 0;
        private void Awake()
        {
            _characterHealthBarOriginalWidth = _characterHealthBar.sizeDelta.x;
            _characterEnergyBarOriginalWidth = _characterEnergyBar.sizeDelta.x;
        }
        public void UpdateCharacterHealth(int current, int max)
        {
            if(max == 0 || current > max)
            {
                return;
            }
            float value = current;
            value /= max;
            UpdateHealthBar(value);
            UpdateHealthText(current, max);
        }

        public void UpdateCharacterEnergy(int current, int max)
        {
            if (max == 0 || current > max)
            {
                return;
            }
            float value = current;
            value /= max;
            UpdateEnergyBar(value);
            UpdateEnergyText(current, max);
        }
        public void UpdateCharacterAbsorb(int amount)
        {
            float value = (float)amount / _characterHealthBarOriginalWidth;
           _characterAbsorbBar.sizeDelta = new Vector2(value* _characterHealthBarOriginalWidth, _characterHealthBar.sizeDelta.y);
            _characterAbsorbAmount = amount;
        }

        private void UpdateHealthText(int current, int max)
        {
            _healthText.text = current + "/" + max;
        }

        private void UpdateEnergyText(int current, int max)
        {
            _energyText.text = current + "/" + max;
        }

        private void UpdateHealthBar(float value)
        {
            _characterHealthBar.sizeDelta = new Vector2(_characterHealthBarOriginalWidth * value, _characterHealthBar.sizeDelta.y);
        }

        private void UpdateEnergyBar(float value)
        {
            _characterEnergyBar.sizeDelta = new Vector2(_characterEnergyBarOriginalWidth * value, _characterEnergyBar.sizeDelta.y);
        }

    }
}