using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover
{
    public class ScreenTransitionScript : MonoBehaviour
    {
        private GameManager _gameManager;
        private Action _action;
        private bool actionCalled = false;
        private float _maxDuration = 2f;
        private float _currentTime = 0f;

        private SpriteRenderer _spriteRenderer;

        public void Initialize(GameManager gameManager, float duration, Action action)
        {
            _gameManager = gameManager;
            _maxDuration = duration;
            _action = action;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            StartCoroutine(FadeIn());
        }
        
        
        private IEnumerator FadeIn()
        {
            float progress = _currentTime / (0.5f * _maxDuration);
            while (progress <= 0.99f)
            {
                progress = _currentTime / (0.5f * _maxDuration);
                Debug.Log("Progress is :  " + progress);
                _spriteRenderer.color = new Color(0, 0, 0, progress);
                _currentTime += Time.deltaTime;
                yield return null;
            }

            
        }
    }
}