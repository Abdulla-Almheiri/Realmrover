using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover
{
    public class MainMapScript : MonoBehaviour
    {
        private GameManager _gameManager;

        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;

        }

        public void GoToLevel(int index)
        {
            _gameManager.StartLevel(index);
        }
    }
}