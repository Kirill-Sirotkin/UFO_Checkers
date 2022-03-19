using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace UFOCheckers
{
    public class MainMenuManager : MonoBehaviour
    {
        public static MainMenuManager Instance { get; set; }

        [field: SerializeField] private BackgroundManager bg_man;
        [field: SerializeField] private GameObject bg_object;

        [field: SerializeField] private GameObject main_menu_panel;
        [field: SerializeField] private GameObject options_menu_panel;
        [field: SerializeField] private GameObject new_game_menu_panel;
        [field: SerializeField] private GameObject name_enter_menu_panel;

        [field: SerializeField] private GameObject player_name;

        [field: SerializeField] private GameObject connect_game_panel;
        [field: SerializeField] private GameObject host_game_panel;
        [field: SerializeField] private GameObject new_game_buttons_panel;

        [field: SerializeField] private GameObject server_prefab;
        [field: SerializeField] private GameObject client_prefab;

        [field: SerializeField] private TMP_InputField input_field;
        [field: SerializeField] private TMP_InputField name_input_field;

        private GameObject current_active_menu;

        private int server_port;

        //private Server s;
        //private Client c;

        private string current_client_name;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetNameEnterMenu();
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
            print("chosen name: " + current_client_name);

            PlayerPrefs.SetString("Client_name", current_client_name);
            PlayerPrefs.Save();

            player_name.GetComponent<TMP_Text>().text = current_client_name;
            player_name.SetActive(true);
        }
        public void SetMainMenu() 
        {
            if(current_active_menu != null) 
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

            current_active_menu.SetActive(true);

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

            //if (s != null) 
            //{
            //    Destroy(s.gameObject);
            //    s.server.Stop();
            //}
            //if (c != null)
            //{
            //    Destroy(c.gameObject);
            //}
        }
        public void ConnectEnterHost() 
        {
            print("Connectin!!!");

            string hostAddress = input_field.text;

            if (hostAddress == null)
            {
                print("no host address");
                hostAddress = "127.0.0.1";
            }

            print("Host Address: " + hostAddress);

            try 
            {
                //c = Instantiate(client_prefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Client>();
                //c.client_name = current_client_name;
                //c.isHost = false;
                //c.ConnectToServer(hostAddress, 6322);
            }
            catch (Exception e)
            {
                Debug.Log("Connection address error: " + e.Message);
            }
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

            //if (s != null)
            //{
            //    Destroy(s.gameObject);
            //    s.server.Stop();
            //}
            //if (c != null)
            //{
            //    Destroy(c.gameObject);
            //}
        }
        public void HostGame() 
        {

            //try
            //{
            //    s = Instantiate(server_prefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Server>();
            //    print("Hostin!!!");
            //    s.Init();
            //    server_port = s.port;

            //    c = Instantiate(client_prefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Client>();
            //    c.client_name = current_client_name + " (Host)";
            //    c.isHost = true;
            //    c.ConnectToServer("127.0.0.1", server_port);
            //}
            //catch (Exception e)
            //{
            //    Debug.Log("Server creation error: " + e.Message);
            //}
        }

        public void StartGame() 
        {
            bg_man.enabled = false;
            bg_object.SetActive(false);
            SceneManager.LoadScene(1);
        }
    }
}
