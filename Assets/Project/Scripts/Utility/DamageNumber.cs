using UnityEngine;
using TMPro;

namespace Realmrover
{
    public class DamageNumber : MonoBehaviour
    {
        private int _value = 0;
        public float Speed = 2f;

        private TextMeshProUGUI _textMP;
        void Start()
        {
            _textMP = GetComponent<TextMeshProUGUI>();
            _textMP.text = _value.ToString();
            Destroy(this.gameObject, 5f);
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(new Vector3(0f, Speed*Time.deltaTime, 0f));
        }

        public void Initialize(int value, Vector2 location)
        {
            _value = value;
            transform.Translate(location);
        }
    }
}