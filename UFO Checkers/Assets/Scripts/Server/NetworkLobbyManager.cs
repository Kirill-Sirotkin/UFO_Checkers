using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Mirror.FizzySteam;
using System.Linq;

namespace UFOCheckers
{
    
    public class NetworkLobbyManager : NetworkManager
    {
        
        //[Header("Start Scene Manager")]
        //[field: SerializeField] private StartScreenManager start_screen_manager = null;
        [Header ("Main Menu Scene")]
        [Scene] [field: SerializeField] private string main_menu_scene = string.Empty;
        //[field: SerializeField] private GameObject main_menu_objects = null;
        [Header("Lobby Player")]
        [field: SerializeField] private NetworkLobbyPlayer lobby_player_prefab = null;
        [field: SerializeField] private int min_players = 2;
        [Header("Game Player")]
        [field: SerializeField] private GameLevelPlayer game_level_player_prefab = null;
        [field: SerializeField] private PlayerInputMP player_input = null;
        //[field: SerializeField] private GameObject coordinator_prefab = null;
        [Header("Steamworks")]
        [field: SerializeField] private FizzySteamworks fizzy_script = null;

        public GameLevelManager Game_level_manager { get; set; }
        public List<NetworkLobbyPlayer> lobby_players { get; } = new List<NetworkLobbyPlayer>();
        public List<GameLevelPlayer> game_players { get; } = new List<GameLevelPlayer>();
        public List<GameObject> player_objects { get; } = new List<GameObject>();

        public Camera main_camera { get; set; }

        public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

        private StartScreenManager start_manager;
        private StartScreenManager StartManager
        {
            get
            {
                if (start_manager != null) { return start_manager; }
                return start_manager = GameObject.FindGameObjectWithTag("StartScreenManager").GetComponent<StartScreenManager>();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                //print("p1: " + Game_level_manager.P1_mouse_pos);
                //print("p2: " + Game_level_manager.P2_mouse_pos);
            }
        }

        public override void OnStartClient()
        {
            //base.OnStartClient();

            var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

            foreach (var prefab in spawnablePrefabs) 
            {
                NetworkClient.RegisterPrefab(prefab);
            }
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            //Do on client connect thing
            StartManager.HandlePlayerConnect();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            //Do on client disconnect thing
            if (SceneManager.GetActiveScene().name == "Test 1") { return; }
            StartManager.HandlePlayerDisconnect();
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            if(numPlayers >= maxConnections) 
            {
                conn.Disconnect();
                return;
            }

            if(SceneManager.GetActiveScene().path != main_menu_scene) 
            {
                conn.Disconnect();
                return;
            }
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            if (SceneManager.GetActiveScene().path == main_menu_scene) 
            {
                bool isHost = lobby_players.Count == 0;

                NetworkLobbyPlayer lobby_player_instance = Instantiate(lobby_player_prefab);

                lobby_player_instance.IsHost = isHost;

                NetworkServer.AddPlayerForConnection(conn, lobby_player_instance.gameObject);
            }
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (conn.identity != null)
            {
                var player = conn.identity.GetComponent<NetworkLobbyPlayer>();

                lobby_players.Remove(player);

                NotifyPlayersOfReadyState();
            }

            if (SceneManager.GetActiveScene().name == "Test 1") 
            {
                SceneManager.LoadScene(1);

                //foreach(var p in game_players) 
                //{
                //    p.CQuitMatch();
                //    p.connectionToServer.Disconnect();
                //}

                StopHost();
            }

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            lobby_players.Clear();

            base.OnStopServer();
        }

        public void NotifyPlayersOfReadyState() 
        {
            foreach (var player in lobby_players) 
            {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }

        private bool IsReadyToStart() 
        {
            if(numPlayers < min_players) 
            {
                return false;
            }

            foreach(var player in lobby_players) 
            {
                if (!player.IsReady) 
                {
                    return false;
                }
            }

            return true;
        }

        public void StartGame() 
        {
            if(SceneManager.GetActiveScene().path == main_menu_scene) 
            {
                if(!IsReadyToStart()) { return; }

                ServerChangeScene("Test 1");
            }
        }

        public override void ServerChangeScene(string newSceneName)
        {
            if (SceneManager.GetActiveScene().path == main_menu_scene && newSceneName.StartsWith("Test"))
            {
                for (int i = lobby_players.Count - 1; i >= 0; i--)
                //for (int i = 0; i < lobby_players.Count; i++)
                {
                    var conn = lobby_players[i].connectionToClient;
                    var gamePlayerInstance = Instantiate(game_level_player_prefab);
                    //gamePlayerInstance.SetDisplayName(lobby_players[i].DisplayName);
                    //print("Chengin....");

                    gamePlayerInstance.displayName = lobby_players[i].DisplayName;

                    if (i != 0) 
                    {
                        gamePlayerInstance.playerNum = UFOPlayer.P2;
                    }
                    else
                    {
                        gamePlayerInstance.playerNum = UFOPlayer.P1;
                    }

                    NetworkServer.Destroy(conn.identity.gameObject);

                    NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject, true);
                    gamePlayerInstance.gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
                    game_players.Add(gamePlayerInstance);
                    //print("Last added player: " + game_players[game_players.Count - 1].displayName);
                    //if (game_players.Count > 0) { print("NON NULL GAME PLAYERS !!!!!!!!!! (before scene switch)"); }
                }
            }

            if (newSceneName.StartsWith("Main")) 
            {

            }

            base.ServerChangeScene(newSceneName);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            NetworkServer.SpawnObjects();

            Game_level_manager = GameObject.FindGameObjectWithTag("GameLevelManager").GetComponent<GameLevelManager>();

            //Game_level_manager.RpcUpdateCamera(game_players);

            //foreach (var player in game_players)
            //{
            //    if (!player.hasAuthority || !player.isLocalPlayer)
            //    {
            //        continue;
            //    }

            //    var conn = player.connectionToClient;
            //    var input_inst = Instantiate(player_input);
            //    NetworkServer.Spawn(input_inst.gameObject, conn);
            //    input_inst.player_num = player.playerNum;
            //}

            main_camera = Camera.main;

            base.OnServerSceneChanged(sceneName);
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);

            if (conn.identity.gameObject.GetComponent<GameLevelPlayer>() == null) { Debug.Log("no game level player found"); return; }

            conn.identity.gameObject.GetComponent<GameLevelPlayer>().CmdSendReadyToken();
        }

        public string GetSteamID64() 
        {
            return fizzy_script.SteamUserID.ToString();
        }
    }
}
