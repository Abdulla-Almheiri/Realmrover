using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Realmrover
{
    public class LevelNameBoxScript : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _titleText;
        [SerializeField]
        private Image _tooltipBackground;
        [SerializeField]
        private Image _tooltipFrame;

        private GameManager _gameManager;

        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                FadeIn(1f);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                FadeOut(1f);
            }
        }
        public void FadeOut(float duration)
        {
            StartCoroutine(Fade(duration, false));
        }

        public void FadeIn(float duration)
        {
            StartCoroutine(Fade(duration, true));
        }

        private IEnumerator Fade(float duration, bool fadeIn)
        {
            if(duration == 0f)
            {
                ApplyAlpha(fadeIn ? 1f : 0f);
            }

            float originalDuration = duration;
            while(duration > 0)
            {
                ApplyAlpha(fadeIn ? 1 - (duration / originalDuration) : duration / originalDuration);
                duration -= Time.deltaTime;
                yield return null;
            }
        }

        private void ApplyAlpha(float amount)
        {
            _titleText.color = new Color(_titleText.color.r, _titleText.color.g, _titleText.color.b, amount);
            _tooltipBackground.color = new Color(_tooltipBackground.color.r, _tooltipBackground.color.g, _tooltipBackground.color.b, amount);
            _tooltipFrame.color = new Color(_tooltipFrame.color.r, _tooltipFrame.color.g, _tooltipFrame.color.b, amount);

        }

        public void SetName(string name)
        {
            _titleText.text = name;
        }
    }
}