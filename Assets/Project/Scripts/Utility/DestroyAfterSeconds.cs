using UnityEngine;

namespace Realmrover
{
    public class DestroyAfterSeconds : MonoBehaviour
    {
        public float Seconds = 5.0f;
        // Start is called before the first frame update
        void Start()
        {
            Destroy(this.gameObject, Seconds);
        }

    }
}