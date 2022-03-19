using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

namespace UFOCheckers
{
    public class NetworkInGamePlayer : NetworkBehaviour
    {
        [SyncVar]
        private string displayName = "...";

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

        //public override void OnStartAuthority()
        //{
        //    CmdSetDisplayName(StartScreenManager.current_client_name);

        //    lobbyUICanvas.SetActive(true);

        //    base.OnStartAuthority();
        //}

        public override void OnStartClient()
        {
            DontDestroyOnLoad(gameObject);

            //Lobby.game_players.Add(this);

            base.OnStartClient();
        }

        public override void OnStopClient()
        {
            //Lobby.game_players.Remove(this);

            base.OnStopClient();
        }

        [Server]
        public void SetDisplayName(string displayName) 
        {
            this.displayName = displayName;
        }
    }
}
