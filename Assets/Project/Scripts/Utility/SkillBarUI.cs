using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Realmrover
{
    public class SkillBarUI : MonoBehaviour
    {
        public GameManager GameManager;
        public List<Image> Images;

        // Start is called before the first frame update
        void Start()
        {
            UpdateSkillUI();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateSkillUI()
        {
            int index = 0;
            var player = GameManager.CurrentPlayerCharacter();

            foreach(Image img in Images)
            {
                if(player.Skills().Count == 0 || index >= player.Skills().Count)
                {
                    Debug.Log("Count issues");
                    return;
                }

                if (player.Skills()[index].Icon != null)
                {
                    img.sprite = player.Skills()[index].Icon;
                }

                index++;
            }
        }
    }
}