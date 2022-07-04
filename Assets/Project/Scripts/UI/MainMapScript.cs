using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Realmrover
{
    public class MainMapScript : MonoBehaviour
    {
        [SerializeField] private TooltipScript _levelTooltip;
        [SerializeField] List<Button> _levelNodes;
        private const int MaxLevel = 3;
        private GameManager _gameManager;

        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;

        }

        public void GoToLevel(int index)
        {
            if (_gameManager.GameState == GameState.MAIN_MAP || _gameManager.GameState == GameState.ENTERING_MAIN_MAP)
            {
                _gameManager.PlayClickSound();
                _gameManager.LevelSelected(index);
            }
        }

        public void UpdateLayout(int levelReached)
        {
            if(levelReached > MaxLevel)
            {
                return;
            }

            for(int i = 0; i<levelReached; i++)
            {
                _levelNodes[i].gameObject.SetActive(true);
            }
        }

        public void Restart()
        {
            for(int i = 1; i<MaxLevel; i++)
            {
                _levelNodes[i].gameObject.SetActive(false);
            }
        }

        public void HoverOverLevel(int index)
        {
            _levelTooltip.gameObject.SetActive(true);
            _levelTooltip.SetTooltip(_gameManager.GameLevelByIndex(index));
        }

        public void HideLevelTooltip()
        {
            _levelTooltip.gameObject.SetActive(false);
        }
    }
}