using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.SceneManagement;

namespace UFOCheckers
{
    public class GameLevelManager : NetworkBehaviour
    {
        private bool game_started = false;

        [field: SerializeField] private TMP_Text manager_counter_text;
        [field: SerializeField] public GameObject[] playable_tiles { get; set; }
        [field: SerializeField] public GameObject[] p1_UFOs { get; set; }
        [field: SerializeField] public GameObject[] p2_UFOs { get; set; }
        [field: SerializeField] public Color p1_color { get; set; }
        [field: SerializeField] public Color p2_color { get; set; }
        [field: SerializeField] public TMP_Text p1_name_text { get; set; }
        [field: SerializeField] public TMP_Text p2_name_text { get; set; }
        [field: SerializeField] public GameObject p1_arrow { get; set; }
        [field: SerializeField] public GameObject p2_arrow { get; set; }
        [field: SerializeField] public GameObject turn_switch_obj { get; set; }
        [field: SerializeField] public TMP_Text p1_score { get; set; }
        [field: SerializeField] public TMP_Text p2_score { get; set; }
        [field: SerializeField] public GameObject pause_panel { get; set; }
        [field: SerializeField] public GameObject end_panel { get; set; }
        [field: SerializeField] public TMP_Text end_name { get; set; }
        [field: SerializeField] public TMP_Text end_text_1 { get; set; }
        [field: SerializeField] public GameObject stalemate_obj { get; set; }
        [field: SerializeField] public TMP_Text stalemate_text { get; set; }

        private int counter;
        private bool isPauseActive = false;

        private NetworkLobbyManager lobby;
        private NetworkLobbyManager Lobby
        {
            get
            {
                if (lobby != null) { return lobby; }
                return lobby = NetworkManager.singleton as NetworkLobbyManager;
            }
        }

        public void UpdateCounter(int c) 
        {
            counter = c;
            manager_counter_text.text = counter.ToString();
        }

        private void OnEnable()
        {
            //SayHello();
            GameObject[] objs = GameObject.FindGameObjectsWithTag("GameLevelPlayer");

            foreach (var obj in objs) 
            {
                if (obj.GetComponent<GameLevelPlayer>().playerNum == UFOPlayer.P1) 
                {
                    p1_name_text.text = obj.GetComponent<GameLevelPlayer>().displayName;
                }
                else 
                {
                    p2_name_text.text = obj.GetComponent<GameLevelPlayer>().displayName;
                }

                if (obj.GetComponent<NetworkIdentity>().isServer) 
                {
                    //SendReadyToken(obj);
                }
            }

            PopupTurnSwitch(p1_name_text.text);
        }

        public void GameOverMenu(string n, string text1) 
        {
            end_text_1.text = text1;
            end_name.text = n;
            end_panel.SetActive(true);
        }

        public void PauseMenu() 
        {
            isPauseActive = !isPauseActive;
            pause_panel.SetActive(isPauseActive);
        }

        [Client]
        public void SwitchArrows() 
        {
            p1_arrow.SetActive(!p1_arrow.activeSelf);
            p2_arrow.SetActive(!p2_arrow.activeSelf);
        }

        [Client]
        public void PopupTurnSwitch(string n) 
        {
            if (turn_switch_obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("TurnSwitchText_Popup"))
            {
                turn_switch_obj.GetComponent<Animator>().Play("TurnSwitchText_Idle");
            }
            turn_switch_obj.GetComponent<TMP_Text>().text = n + "'s turn!";
            turn_switch_obj.GetComponent<Animator>().SetTrigger("Popup");
        }

        private void SendReadyToken(GameObject server_obj)
        {
            server_obj.GetComponent<GameLevelPlayer>().CmdSendReadyToken();
        }

        [Server]
        public void SayHello() 
        {
            Debug.Log("hello");
        }

        [Client]
        public void QuitMatch()
        {
            //if (isLocalPlayer)
            //{
            //    CmdQuitMatchSceneChange();
            //}


            Lobby.StopHost();
            Lobby.StopClient();
            SceneManager.LoadScene(1);
        }

        [Command]
        public void CmdQuitMatchSceneChange() 
        {
            RpcQuitMatchSceneChange();
        }

        [ClientRpc]
        public void RpcQuitMatchSceneChange()
        {
            SceneManager.LoadScene(1);
        }

        [Command]
        public void CmdStopGame()
        {
            Lobby.StopHost();
        }
    }
}
