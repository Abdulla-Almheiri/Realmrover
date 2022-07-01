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
        private Button _battleControlButton;

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
        private List<Skill> _skills;

        private GameManager _gameManager;
        private BattleControlButtonTextType _buttonState = BattleControlButtonTextType.INITIAL;

        private void Update()
        {
            if(_buttonState == BattleControlButtonTextType.ENEMY_TURN)
            {

            }
        }
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
            _playerResources.UpdateCharacterAbsorb(character.AbsorbAmount());
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
            _skills = skills;
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
            _gameManager.BattleControlButtonPressed();
        }

        public void SetButtonText(BattleControlButtonTextType textType)
        {
            /*if(textType == _buttonState)
            {
                return;
            }*/

            textType = _buttonState;
            switch (textType)
            {
                case BattleControlButtonTextType.END_TURN:
                    _battleControlText.text = "End Turn";
                    _battleControlButton.interactable = true;
                    break;
                case BattleControlButtonTextType.BACK_TO_MAIN_MAP:
                    _battleControlText.text = "Main Map";
                    _battleControlButton.interactable = true;
                    break;
                case BattleControlButtonTextType.NEXT_BATTLE:
                    _battleControlText.text = "Next Battle";
                    _battleControlButton.interactable = true;
                    break;
                case BattleControlButtonTextType.ENEMY_TURN:
                    _battleControlText.text = "Enemy Turn";
                    _battleControlButton.interactable = false;
                    break;
                case BattleControlButtonTextType.EMPTY:
                    _battleControlText.text = "";
                    _battleControlButton.interactable = false;
                    break;
                default:
                    break;
            }
        }

        public void MouseEnterSkill(int index)
        {
            Debug.Log("Mouse Entered  :  " + index);
            if(index < 0 || index >= _skills.Count)
            {
                return;
            }
            _gameManager.ShowTooltip(_skills[index]);
        }

        public void MouseLeaveSkill(int index)
        {
            Debug.Log("Mouse Left  :  " + index);
            _gameManager.HideTooltip();
        }

        public void ToggleEnemyResources(bool value)
        {
            _enemyResources.gameObject.SetActive(value);
        }
    }

    public enum BattleControlButtonTextType
    {
        INITIAL,
        END_TURN,
        ENEMY_TURN,
        NEXT_BATTLE,
        BACK_TO_MAIN_MAP,
        EMPTY
    
    }
}