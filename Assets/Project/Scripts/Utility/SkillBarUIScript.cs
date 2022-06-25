using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Realmrover
{
    public class SkillBarUIScript : MonoBehaviour
    {
        private GameManager _gameManager;
        [SerializeField]
        private Sprite _defaultSprite;
        [SerializeField]
        private List<SpriteRenderer> _skillSprites;
        [SerializeField]
        private List<SpriteRenderer> _rechargeSprites;

        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
        }
        public void UpdateSkillUI()
        {
            int index = 0;
            var player = _gameManager.CurrentPlayerCharacter();
            if(player == null)
            {
                return;
            }

            foreach(SpriteRenderer sprite in _skillSprites)
            {
                if(player.Skills().Count == 0 || index >= player.Skills().Count)
                {
                    Debug.Log("Count issues");
                    return;
                }

                if (player.Skills()[index].Icon != null)
                {
                    sprite.sprite = player.Skills()[index].Icon;
                }

                index++;
            }
        }

        public void UpdateSkillRecharge(int skillIndex, float value)
        {
            if(skillIndex < 0 || skillIndex >= _rechargeSprites.Count)
            {
                return;
            }
            value = Mathf.Clamp(value, 0f, 1f);
            _rechargeSprites[skillIndex].transform.localScale = new Vector3(1, value, 1);
        }

        public void UpdateRechargeAllSkills(float value)
        {
            value = Mathf.Clamp(value, 0f, 1f);

            for(int i = 0; i<7; i++)
            { 
                _rechargeSprites[i].transform.localScale = new Vector3(1, value, 1);
            }
        }

        public void ClearSkillBar()
        {
            foreach(SpriteRenderer sprite in _skillSprites)
            {
                sprite.sprite = _defaultSprite;
            }
        }

        public void AbilityClicked(int index)
        {
            _gameManager.ActivateAbility(index);
        }
    }
}