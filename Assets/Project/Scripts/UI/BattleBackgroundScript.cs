using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Realmrover
{
    public class BattleBackgroundScript : MonoBehaviour
    {
        private GameManager _gameManager;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void SetBackground(GameLevel gameLevel)
        {
            if (gameLevel.Background != null)
            {
                _spriteRenderer.sprite = gameLevel.Background;
            }
        }

    }
}