using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Realmrover
{
    public class EndScreenScript : MonoBehaviour
    {
        public void OpenURL()
        {
            Application.OpenURL("https://forms.gle/Vomx5tmRNX4buDv6A");
        }

        public void BackToMainMenuButton()
        {
            SceneManager.LoadScene(0);
        }
    }
}