using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Realmrover
{
    public class SettingsMenuScript : MonoBehaviour
    {

        [SerializeField]
        private Slider _soundSlider;

        [SerializeField]
        private Slider _musicSlider;

        private float _soundVolume;
        private float _musicVolume;
        private GameManager _gameManager;

        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;

            if(PlayerPrefs.HasKey("SoundVolume"))
            {
                _soundVolume = PlayerPrefs.GetFloat("SoundVolume");
                _soundSlider.value = _soundVolume;
            }
            else
            {
                _soundVolume = _soundSlider.value;
            }

            if (PlayerPrefs.HasKey("MusicVolume"))
            {
                _musicVolume = PlayerPrefs.GetFloat("MusicVolume");
                _musicSlider.value = _musicVolume;
            }
            else
            {
                _musicVolume = _musicSlider.value;
            }
            
        }

        public void SoundUpdated()
        {
            _soundVolume = _soundSlider.value;
        }

        public void MusicUpdated()
        {
            _musicVolume = _musicSlider.value;
        }

        public float SoundVolume()
        {
            return _soundVolume;
        }

        public float MusicVolume()
        {
            return _musicVolume;
        }

        public void ConfirmSettings()
        {
            PlayerPrefs.SetFloat("SoundVolume", _soundVolume);
            PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
            gameObject.SetActive(false);
        }
    }
}