using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover {
    public class MainMenuScript : MonoBehaviour
    {
        private GameManager _gameManager;

        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void NewGameButton()
        {
            _gameManager.NewGameButtonPressed();
        }

        public void SettingsButton()
        {
            _gameManager.ShowSettingsMenu();
        }

        public void QuitButton()
        {
            Application.Quit();
        }
    }
}
