using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;


namespace UFOCheckers
{
    public class StartScreenManager : MonoBehaviour
    {
        [Header("Background")]
        [field: SerializeField] private BackgroundManager bg_man;
        [field: SerializeField] private GameObject bg_object;

        [Header("UI")]
        [field: SerializeField] private GameObject main_menu_panel;
        [field: SerializeField] private GameObject options_menu_panel;
        [field: SerializeField] private GameObject new_game_menu_panel;
        [field: SerializeField] private GameObject name_enter_menu_panel;

        [field: SerializeField] private GameObject player_name;

        [field: SerializeField] private GameObject connect_game_panel;
        [field: SerializeField] private GameObject host_game_panel;
        [field: SerializeField] private GameObject new_game_buttons_panel;

        [Header("Input fields")]
        [field: SerializeField] private TMP_InputField ip_input_field;
        [field: SerializeField] private TMP_InputField name_input_field;

        //[Header("Networking")]
        //[field: SerializeField] private NetworkLobbyManager lobby_manager = null;

        private GameObject current_active_menu;

        private NetworkLobbyManager lobby;
        private NetworkLobbyManager Lobby
        {
            get
            {
                if (lobby != null) { return lobby; }
                return lobby = NetworkManager.singleton as NetworkLobbyManager;
            }
        }

        public static string current_client_name { get; private set; }

        private void Awake()
        {
            SetNameEnterMenu();
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (name_enter_menu_panel.activeSelf)
                {
                    SetName();
                    SetMainMenu();
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                //PlayerPrefs.SetString("Client_name", null);
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        public void SetName()
        {
            string n = name_input_field.text;

            if (n == "")
            {
                n = "unnamed";
            }

            current_client_name = n;
            //print("chosen name: " + current_client_name);

            PlayerPrefs.SetString("Client_name", current_client_name);
            PlayerPrefs.Save();

            player_name.GetComponent<TMP_Text>().text = current_client_name;
            player_name.SetActive(true);
        }
        public void SetMainMenu()
        {
            if (current_active_menu != null)
            {
                current_active_menu.SetActive(false);
            }

            current_active_menu = main_menu_panel;

            current_active_menu.SetActive(true);

            //print("mainmenu active");
        }
        public void SetOptionsMenu()
        {
            if (current_active_menu != null)
            {
                current_active_menu.SetActive(false);
            }

            current_active_menu = options_menu_panel;

            current_active_menu.SetActive(true);

            //print("options active");
        }
        public void SetNewGameMenu()
        {
            if (current_active_menu != null)
            {
                current_active_menu.SetActive(false);
            }

            current_active_menu = new_game_menu_panel;

            if (current_active_menu != null)
            {
                current_active_menu.SetActive(true);
            }

            //print("newgame active");
        }
        public void SetNameEnterMenu()
        {
            if (current_active_menu != null)
            {
                current_active_menu.SetActive(false);
            }

            current_active_menu = name_enter_menu_panel;

            if (PlayerPrefs.GetString("Client_name") != null)
            {
                name_input_field.text = PlayerPrefs.GetString("Client_name");
            }

            player_name.SetActive(false);

            current_active_menu.SetActive(true);

        }
        public void ConnectMenu()
        {
            new_game_buttons_panel.SetActive(false);
            connect_game_panel.SetActive(true);
        }
        public void CancelEnterHost()
        {
            connect_game_panel.SetActive(false);
            new_game_buttons_panel.SetActive(true);
        }
        public void ConnectEnterHost()
        {
            string ipAddress = ip_input_field.text;

            if(ipAddress == "") 
            {
                //print("set default IP address");
                ipAddress = "127.0.0.1";
            }

            Lobby.networkAddress = ipAddress;
            Lobby.StartClient();

            //print("connecting to game");
        }
        public void HostMenu()
        {
            new_game_buttons_panel.SetActive(false);
            host_game_panel.SetActive(true);
        }
        public void CancelHostMenu()
        {
            host_game_panel.SetActive(false);
            new_game_buttons_panel.SetActive(true);
        }
        public void HostGame()
        {
            Lobby.StartHost();

            //print("hosting game");
        }
        public void StartGame()
        {
            bg_man.enabled = false;
            bg_object.SetActive(false);
            SceneManager.LoadScene(1);
        }
        public void SetLobbyScreen()
        {
            current_active_menu.SetActive(false);
            connect_game_panel.SetActive(false);
            host_game_panel.SetActive(false);
            new_game_buttons_panel.SetActive(true);
        }
        public void HandlePlayerConnect() 
        {
            SetLobbyScreen();
        }
        public void HandlePlayerDisconnect()
        {
            SetNewGameMenu();
        }
    }
}
