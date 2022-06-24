using UnityEngine;
using TMPro;

namespace Realmrover
{
    public class DamageNumber : MonoBehaviour
    {
        private int _value = 0;
        public float Speed = 2f;
        private GameManager _gameManager;

        private TextMeshProUGUI _textMP;

        void Update()
        {
            transform.Translate(new Vector3(0f, Speed*Time.deltaTime, 0f));
        }

        public void Initialize(int value, FloatingTextType textType, Vector2 location, GameManager gameManager)
        {
            _gameManager = gameManager;
            _value = value;

            _textMP = GetComponent<TextMeshProUGUI>();
            _textMP.text = value.ToString();
            Destroy(this.gameObject, 5f);

            
            //Update colors here

            transform.Translate(location);
            if(gameManager.TextColorsPreset == null)
            {
                return;
            }

            switch (textType)
            {
                case FloatingTextType.DIRECT_DAMAGE:
                    _textMP.color = gameManager.TextColorsPreset.DirectDamage;
                    break;
                case FloatingTextType.DIRECT_HEAL:
                    _textMP.color = gameManager.TextColorsPreset.DirectHeal;
                    break;
                case FloatingTextType.HEALTH_REGEN:
                    _textMP.color = gameManager.TextColorsPreset.HealthRegen;
                    //transform.Translate(new Vector3(-2f,0,0));
                    break;
                case FloatingTextType.ENERGY_REGEN:
                    _textMP.color = gameManager.TextColorsPreset.EnergyRegen;
                    //transform.Translate(new Vector3(-3f, 0, 0));
                    break;
                case FloatingTextType.HEAL_PER_TURN:
                    _textMP.color = gameManager.TextColorsPreset.HealPerTurn;
                    //transform.Translate(new Vector3(1f, 0, 0));
                    break;
                case FloatingTextType.DAMAGE_PER_TURN:
                    _textMP.color = gameManager.TextColorsPreset.DamagePerTurn;
                    //transform.Translate(new Vector3(2f, 0, 0));
                    break;
                case FloatingTextType.COMPLETE_ABSORB:
                    _textMP.color = gameManager.TextColorsPreset.CompleteAbsorb;
                    break;
                case FloatingTextType.REFLECT:
                    _textMP.color = gameManager.TextColorsPreset.Reflect;
                    break;
                case FloatingTextType.SACRIFICE:
                    _textMP.color = gameManager.TextColorsPreset.Sacrifice;
                    //transform.Translate(new Vector3(-4f, 0, 0));
                    break;
                default:
                    break;
            }
        }
    }
}