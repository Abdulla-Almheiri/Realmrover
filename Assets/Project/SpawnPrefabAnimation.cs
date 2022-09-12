using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover
{
    public class SpawnPrefabAnimation : MonoBehaviour
    {
        public GameObject Animation;
        private int counter = 30*5;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(counter == 0)
            {
                Instantiate(Animation);
                counter--;
            } else if(counter > 0)
            {
                counter--;
            }
        }
    }
}