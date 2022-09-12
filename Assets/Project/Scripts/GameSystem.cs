using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmrover
{
    public abstract class GameSystem
    {
        public GameManager GameManager { get; protected set; }

        public virtual void Initialize(GameManager gameManager)
        {
            GameManager = gameManager;
        }

    }
}