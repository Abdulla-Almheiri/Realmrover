using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

        private GameManager _gameManager;
        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
            _titleText.text = "";
            _mainText.text = "";
            _topLeftText.text = "";
            _topRightText.text = "";
            _bottomLeftText.text = "";
            _bottomRightText.text = "";
        }

        public void SetTooltip(Skill skill)
        {
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
            _titleText.text = template.Name;
            _mainText.text = template.Description;
        }
    }
}