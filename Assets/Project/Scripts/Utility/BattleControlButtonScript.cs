using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Realmrover
{
    public class BattleControlButtonScript : MonoBehaviour
    {
        private GameManager _gameManager;
        [SerializeField]
        private TextMeshProUGUI _textUI;
        private BattleButtonStates _buttonState = BattleButtonStates.END_TURN;
        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
            UpdateUI();
        }

        public void UpdateUI(BattleButtonStates buttonState = BattleButtonStates.END_TURN)
        {
            switch(buttonState)
            {
                case BattleButtonStates.END_TURN:
                    _textUI.text = "End Turn";
                    break;
                case BattleButtonStates.NEXT_BATTLE:
                    _textUI.text = "Next Battle";
                    break;
                case BattleButtonStates.LEAVE:
                    _textUI.text = "Leave";
                    break;
            }
        }

        public void ButtonPressed()
        {
            _gameManager.BattleControlButtonPressed(_buttonState);
        }
    }

    public enum BattleButtonStates
    {
        END_TURN,
        NEXT_BATTLE,
        LEAVE

    }
}