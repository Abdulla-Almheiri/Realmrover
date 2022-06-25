using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Realmrover
{
    public class CharacterHUDScript : MonoBehaviour
    {
        private GameManager _gameManager;
        private Character _character;
        [SerializeField]
        private Image _portraitImage;
        [SerializeField]
        private Slider _healthSlider;
        [SerializeField]
        private TextMeshProUGUI _healthText;
        [SerializeField]
        private Slider _energySlider;
        [SerializeField]
        private TextMeshProUGUI _energyText;

        public void Initialize(GameManager gameManager, Character character)
        {
            _gameManager = gameManager;
            _character = character;
            if (_character.Template().PortraitImage != null)
            {
                _portraitImage.sprite = _character.Template().PortraitImage;
            }

            _healthSlider.value = (float)_character.CurrentHealth() / _character.MaxHealth();
            _energySlider.value = (float)_character.CurrentEnergy() / _character.MaxEnergy();

            _healthText.text = _character.CurrentHealth() + "/" + _character.MaxHealth();
            _energyText.text = _character.CurrentEnergy() + "/" + _character.MaxEnergy();

        }

        public void UpdateStats()
        {
            if(_gameManager == null || _character == null)
            {
                return;
            }

            _healthSlider.value = (float)_character.CurrentHealth() / _character.MaxHealth();
            _energySlider.value = (float)_character.CurrentEnergy() / _character.MaxEnergy();

            _healthText.text = _character.CurrentHealth() + "/" + _character.MaxHealth();
            _energyText.text = _character.CurrentEnergy() + "/" + _character.MaxEnergy();
        }
    }
}
