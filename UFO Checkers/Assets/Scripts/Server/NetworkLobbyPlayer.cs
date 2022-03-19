using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

namespace UFOCheckers
{
    public class NetworkLobbyPlayer : NetworkBehaviour
    {
        
        [Header("UI")]
        [field: SerializeField] private GameObject lobbyUICanvas = null;
        [field: SerializeField] private TMP_Text[] player_name_texts = new TMP_Text[2];
        [field: SerializeField] private GameObject[] player_ready_checks = new GameObject[2];
        [field: SerializeField] private TMP_Text waiting_text = null;
        [field: SerializeField] private GameObject start_button = null;

        [SyncVar(hook = nameof(HandleDisplayNameChanged))]
        public string DisplayName = "...";
        [SyncVar(hook = nameof(HandleWaitingTextChanged))]
        public string waiting_text_string = "Waiting for another player...";
        [SyncVar(hook = nameof(HandleReadyStatusChanged))]
        public bool IsReady = false;

        private const string wait_another = "Waiting for another player...";
        private const string wait_ready = "Waiting to get ready...";
        private const string wait_start = "Waiting for host to start...";

        [field: SerializeField] private TMP_Text SteamID64 = null;

        private bool isHost;

        public bool IsHost
        {
            set
            {
                isHost = value;
            }
        }

        private NetworkLobbyManager lobby;

        private NetworkLobbyManager Lobby
        {
            get
            {
                if (lobby != null) { return lobby; }
                return lobby = NetworkManager.singleton as NetworkLobbyManager;
            }
        }

        //private void Awake()
        //{
        //    DontDestroyOnLoad(gameObject);
        //}

        public override void OnStartAuthority()
        {
            CmdSetDisplayName(StartScreenManager.current_client_name);

            lobbyUICanvas.SetActive(true);
            SteamID64.text = "ID64: " + Lobby.GetSteamID64();

            base.OnStartAuthority();
        }

        public override void OnStartClient()
        {
            Lobby.lobby_players.Add(this);

            waiting_text_string = wait_another;
            UpdateDisplay();

            base.OnStartClient();
        }

        public override void OnStopClient()
        {
            Lobby.lobby_players.Remove(this);

            UpdateDisplay();

            base.OnStopClient();
        }

        public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
        public void HandleWaitingTextChanged(string oldValue, string newValue) => UpdateDisplay();
        public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

        private void UpdateDisplay()
        {
            //print("display updated: " + StartScreenManager.current_client_name);

            if (!hasAuthority)
            {
                if(Lobby.lobby_players.Count > 0)
                {
                    //print("lobby players count: " + Lobby.lobby_players.Count);

                    foreach (var player in Lobby.lobby_players)
                    {
                        if (player.hasAuthority)
                        {
                            player.UpdateDisplay();
                            break;
                        }
                    }
                }

                return;
            }

            for (int i = 0; i < player_name_texts.GetLength(0); i++)
            {
                player_name_texts[i].text = "...";
                player_ready_checks[i].SetActive(false);
            }

            for (int i = 0; i < player_name_texts.GetLength(0); i++)
            {
                if (i < Lobby.lobby_players.Count)
                {
                    player_name_texts[i].text = Lobby.lobby_players[i].DisplayName;
                    player_ready_checks[i].SetActive(Lobby.lobby_players[i].IsReady);
                }
            }

            if (isHost) 
            {
                if(Lobby.lobby_players.Count > 1 && waiting_text_string == wait_another) 
                {
                    waiting_text_string = wait_ready;
                }
            }
            waiting_text.text = waiting_text_string;

            if (!isHost)
            {
                if (Lobby.lobby_players.Count > 1)
                {
                    waiting_text.text = Lobby.lobby_players[0].waiting_text_string;
                }
            }
            //if (Lobby.lobby_players.Count > 0) 
            //{
            //    if (Lobby.lobby_players.Count > 1)
            //    {
            //        if (isHost)
            //        {
            //            waiting_text_string = "Waiting to get ready...";
            //            waiting_text.text = waiting_text_string;
            //        }
            //        else
            //        {
            //            waiting_text.text = Lobby.lobby_players[0].waiting_text_string;
            //        }

            //        return;
            //    }

            //    if (isHost)
            //    {
            //        if(start_button.activeSelf == true)
            //        {
            //            waiting_text_string = "Waiting for host to start...";
            //        }
            //        else
            //        {
            //            waiting_text_string = "Waiting for another player...";
            //        }
            //        waiting_text.text = waiting_text_string;
            //    }
            //    else
            //    {
            //        waiting_text.text = Lobby.lobby_players[0].waiting_text_string;
            //    }
            //}
        }

        public void HandleReadyToStart(bool readyToStart)
        {
            if (!isHost) { return; }

            if (readyToStart) 
            {
                //print("host can start");
                start_button.SetActive(true);
                waiting_text_string = wait_start;
                //waiting_text.text = "Waiting for Host to start...";
            }
            else 
            {
                //print("host cannot start");
                start_button.SetActive(false);
                if (Lobby.lobby_players.Count > 1)
                {
                    waiting_text_string = wait_ready;
                }
                else 
                {
                    waiting_text_string = wait_another;
                }
                //waiting_text.text = "Waiting to get ready...";
            }
        }

        public void DisconnectClient()
        {
            if (isHost)
            {
                Lobby.StopHost();
                //print("LOBBY: stopped host");
                return;
            }

            Lobby.StopClient();
            //print("LOBBY: stopped client");

        }

        [Command]
        private void CmdSetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        [Command]
        public void CmdReadyUp()
        {
            IsReady = !IsReady;

            Lobby.NotifyPlayersOfReadyState();
        }

        [Command]
        public void CmdStartGame()
        {
            if (Lobby.lobby_players[0].connectionToClient != connectionToClient) { return; }

            //startgame
            Lobby.StartGame();
        }
        
    }
}
