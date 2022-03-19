using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UFOCheckers
{
    public class PauseMenu : MonoBehaviour
    {
        [field: SerializeField] private GameObject pause_menu_panel;

        private bool isPaused = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) 
            {
                PauseGame();
            }
        }

        public void PauseGame() 
        {
            if (!isPaused)
            {
                pause_menu_panel.SetActive(true);
                isPaused = true;
                return;
            }

            pause_menu_panel.SetActive(false);
            isPaused = false;
        }
    }
}
