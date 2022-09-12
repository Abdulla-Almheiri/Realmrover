using UnityEngine;
using TMPro;

namespace Realmrover
{
    public class DamageNumber : MonoBehaviour
    {
        private int _value = 0;
        public float Speed = 2f;
        private  Vector2 _direction = new Vector2(0, 1f);
        private GameManager _gameManager;

        private TextMeshProUGUI _textMP;

        void Update()
        {
            transform.Translate(new Vector3(_direction.x*Time.deltaTime*Speed, _direction.y*Speed*Time.deltaTime));

            if(Input.GetKeyUp(KeyCode.B))
            {
                _textMP.alpha = 0f;
            }

            if (Input.GetKeyUp(KeyCode.N))
            {
                _textMP.alpha = 1f;
            }
        }

        public void Initialize(int value, FloatingTextType textType, Vector2 location, GameManager gameManager)
        {
            _gameManager = gameManager;
            _value = value;
            transform.Translate(location);
            _textMP = GetComponent<TextMeshProUGUI>();
            _textMP.text = value.ToString();
            Destroy(this.gameObject, 5f);
            
            
            //Update colors here

            
            if(gameManager.TextColorsPreset == null)
            {
                return;
            }

            switch (textType)
            {
                case FloatingTextType.DIRECT_DAMAGE:
                    _textMP.color = gameManager.TextColorsPreset.DirectDamage;
                    _textMP.fontSize += 8;
                    break;
                case FloatingTextType.DIRECT_HEAL:
                    _textMP.color = gameManager.TextColorsPreset.DirectHeal;
                    _textMP.fontSize += 4;
                    break;
                case FloatingTextType.HEALTH_REGEN:
                    _textMP.color = gameManager.TextColorsPreset.HealthRegen;
                    _textMP.fontSize -= 4;
                    break;
                case FloatingTextType.ENERGY_REGEN:
                    _textMP.color = gameManager.TextColorsPreset.EnergyRegen;
                    _textMP.fontSize -= 4;
                    break;
                case FloatingTextType.HEAL_PER_TURN:
                    _textMP.color = gameManager.TextColorsPreset.HealPerTurn;
                    _textMP.fontSize -= 2;
                    break;
                case FloatingTextType.DAMAGE_PER_TURN:
                    _textMP.color = gameManager.TextColorsPreset.DamagePerTurn;
                    _textMP.fontSize += 2;
                    break;
                case FloatingTextType.COMPLETE_ABSORB:
                    _textMP.color = gameManager.TextColorsPreset.CompleteAbsorb;
                    _textMP.fontSize += 10;
                    break;
                case FloatingTextType.REFLECT:
                    _textMP.color = gameManager.TextColorsPreset.Reflect;
                    _textMP.fontSize += 6;
                    break;
                case FloatingTextType.SACRIFICE:
                    _textMP.color = gameManager.TextColorsPreset.Sacrifice;
                    _textMP.fontSize += 2;
                    break;
                default:
                    break;
            }
        }
    }
}