using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Realmrover
{
    public class BattleScreenScript : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _battleControlText;

        [SerializeField]
        private CharacterResourcesUIScript _playerResources;

        [SerializeField]
        private CharacterResourcesUIScript _enemyResources;

        [SerializeField]
        private GameObject _battleBackgroundPrefab;
        private SpriteRenderer _battleBackground;

        [SerializeField]
        private Sprite _defaultBattleBackground;
        

        [SerializeField]
        private Sprite _defaultSkillIcon;
        [SerializeField]
        private Image[] _skillIcons = new Image[7];
        private Button[] _skillButtons = new Button[7];

        private GameManager _gameManager;

        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
            for (int i = 0; i < _skillIcons.Length; i++)
            {
                _skillButtons[i] = _skillIcons[i].GetComponent<Button>();
            }

            _battleBackground = Instantiate(_battleBackgroundPrefab).GetComponent<SpriteRenderer>();
            _battleBackground.sprite = _defaultBattleBackground;
        }

        public void UpdatePlayerResources(Character character)
        {
            _playerResources.UpdateCharacterHealth(character.CurrentHealth(), character.MaxHealth());
            _playerResources.UpdateCharacterEnergy(character.CurrentEnergy(), character.MaxEnergy());
        }

        public void UpdateEnemyResources(Character character)
        {
            _enemyResources.UpdateCharacterHealth(character.CurrentHealth(), character.MaxHealth());
            _enemyResources.UpdateCharacterEnergy(character.CurrentEnergy(), character.MaxEnergy());
        }

        public void UpdatePlayerSkillBar(Character player)
        {
            var skills = player.Skills();
            if (skills == null || skills.Count == 0)
            {
                return;
            }
            for (int i = 0; i < _skillIcons.Length && i < skills.Count; i++)
            {
                UpdateSkill(i, skills[i]);
            }
        }
        public void UpdateBackground(Sprite newSprite)
        {
            if(newSprite == null)
            {
                return;
            }    

            _battleBackground.sprite = newSprite;
        }

        private void UpdateSkill(int index, Skill skill)
        {
            if (index < 0 || index > _skillIcons.Length)
            {
                return;
            }
            if (skill.Icon != null)
            {
                _skillIcons[index].sprite = skill.Icon;
            } else
            {
                _skillIcons[index].sprite = _defaultSkillIcon;
            }
        }

        public void ToggleAbility(int index, bool value)
        {
            if(index >= _skillButtons.Length)
            {
                return;
            }

            _skillButtons[index].interactable = value;
        }

        public void ActivateSkillFromClick(int index)
        {
            _gameManager.ActivateAbility(index);
        }

        public void BattleControlButtonPressed()
        {
            _gameManager.EndTurnButtonPress();
        }

        public void SetButtonText(BattleControlButtonTextType textType)
        {
            switch (textType)
            {
                case BattleControlButtonTextType.END_TURN:
                    _battleControlText.text = "End Turn";
                    break;
                case BattleControlButtonTextType.BACK_TO_MAIN_MAP:
                    _battleControlText.text = "Main Map";
                    break;
                case BattleControlButtonTextType.NEXT_BATTLE:
                    _battleControlText.text = "Next Battle";
                    break;
                default:
                    break;
            }
        }

    }

    public enum BattleControlButtonTextType
    {
        END_TURN,
        NEXT_BATTLE,
        BACK_TO_MAIN_MAP,
    
    }
}