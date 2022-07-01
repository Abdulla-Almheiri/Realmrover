using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Realmrover
{
    public class TooltipScript : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _mainText;
        [SerializeField] private TMP_Text _topLeftText;
        [SerializeField] private TMP_Text _topRightText;
        [SerializeField] private TMP_Text _bottomLeftText;
        [SerializeField] private TMP_Text _bottomRightText;
        [SerializeField] private Button _nextButton;

        private GameManager _gameManager;
        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
            Clear();
        }

        public void SetTooltip(Skill skill)
        {
            if(skill == null)
            {
                return;
            }
            Clear();
            _titleText.text = skill.Name;
            _mainText.text = skill.Description;
            _bottomLeftText.text =  skill.EnergyCost + " energy";
        }

        public void SetTooltip(Character character)
        {
            
            var template = character.Template();
            if(template == null)
            {
                return;
            }
            Clear();
            _titleText.text = template.Name;
            _mainText.text = template.Description;
        }

        public void SetTooltip(string message)
        {
            Clear();
            _mainText.text = message;
            _nextButton.gameObject.SetActive(true);
        }

        public void Clear()
        {
            _titleText.text = "";
            _mainText.text = "";
            _topLeftText.text = "";
            _topRightText.text = "";
            _bottomLeftText.text = "";
            _bottomRightText.text = "";
            _nextButton.gameObject.SetActive(false);
        }

        public void NextButtonPressed()
        {
            _gameManager.MessageNextButtonPressed();
        }
    }
}