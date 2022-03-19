using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UFOCheckers
{
    public class GameLevelPlayer : NetworkBehaviour
    {
        [SyncVar]
        public string displayName;
        [SyncVar]
        public UFOPlayer playerNum;

        #region Successful counter update
        [field: SerializeField] private TMP_Text counter_text;

        private static int server_counter = 0;
        #endregion

        #region Successful mouse click message

        [SyncVar(hook = nameof(HandleUpdateMouseClick))]
        public Vector3 mouse_position = -Vector3.one;

        #endregion

        #region Server game logic variables

        private GameObject selection = null;
        private GameObject prev_selection = null;
        private GameObject ufo_to_attack = null;

        private static UFOPlayer turn_tracker = UFOPlayer.P1;
        private static SelectionState selection_state = SelectionState.NO_SELECTION;

        private List<GameObject> selected_tiles = new List<GameObject>();

        private Vector3 up_dir = new Vector3(0f, 6.76f, 0f);
        private Vector3 down_dir = new Vector3(0f, -6.76f, 0f);
        private Vector3 right_dir = new Vector3(11.72f, 0f, 0f);
        private Vector3 left_dir = new Vector3(-11.72f, 0f, 0f);

        private static Color tile_selection_color = Color.white;

        private int p_1_counter = 0;
        private int p_2_counter = 0;

        private Vector2[] P1_queen_tiles = new Vector2[] { new Vector2(-35.16f, 27.04f), new Vector2(-23.44f, 33.8f), new Vector2(-11.72f, 40.56f), new Vector2(0f, 47.32f) };
        private Vector2[] P2_queen_tiles = new Vector2[] { new Vector2(0f, 0f), new Vector2(11.72f, 6.76f), new Vector2(23.44f, 13.52f), new Vector2(35.16f, 20.28f) };

        private static bool just_queens_left = false;
        private static int stalemate_counter = 4;

        #endregion

        #region Server UI variables

        private List<string> player_names = new List<string>();
        private List<UFOPlayer> player_nums = new List<UFOPlayer>();

        private static int connections = 0;

        #endregion

        private GameLevelManager manager;
        private GameLevelManager Manager 
        {
            get 
            {
                if (manager != null) { return manager; }
                return manager = GameObject.FindGameObjectWithTag("GameLevelManager").GetComponent<GameLevelManager>();
            }
        }

        private Camera main_camera;
        private Camera Main_camera 
        {
            get 
            {
                if (main_camera != null) { return main_camera; }
                return main_camera = Camera.main;
            }
        }

        #region DontDelete

        private NetworkLobbyManager lobby;
        private NetworkLobbyManager Lobby
        {
            get
            {
                if (lobby != null) { return lobby; }
                return lobby = NetworkManager.singleton as NetworkLobbyManager;
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            DontDestroyOnLoad(gameObject);
        }

        public override void OnStopClient()
        {
            SceneManager.LoadScene(1);

            base.OnStopClient();
        }

        #endregion

        private void Update()
        {
            if (!hasAuthority || !isLocalPlayer) { return; }

            if (Input.GetKeyDown(KeyCode.X)) 
            {
                //CmdDoSmth();
                //CmdSwitchTurn();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0)) 
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    mouse_position = Main_camera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0f, 0f, -10f));
                    CmdSendMouseClick(displayName, playerNum, mouse_position);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape)) 
            {
                Manager.PauseMenu();
            }
        }

        #region Successful counter update

        [Command]
        public void CmdDoSmth() 
        {
            //ServerUpdateCounter();
        }

        [ClientRpc]
        public void RpcUpdateClientManager(int c) 
        {
            UpdateClientManager(c);
        }

        [Server]
        public void ServerUpdateCounter(int c, UFOPlayer p_num) 
        {
            //counter++;
            //server_counter++;
            //RpcUpdateClientManager(server_counter);
            RpcUpdateClientCounters(c, p_num);
        }

        //public void HandleUpdateCounter(int oldValue, int newValue) => UpdateCounter();

        [ClientRpc]
        public void RpcUpdateClientCounters(int c, UFOPlayer p_num) 
        {
            UpdateCounter(c, p_num);
        }

        [Client]
        public void UpdateCounter(int c, UFOPlayer p_num) 
        {
            if(p_num == UFOPlayer.P1) 
            {
                Manager.p1_score.text = c.ToString();
            }
            else 
            {
                Manager.p2_score.text = c.ToString();
            }
        }

        [Client]
        public void UpdateClientManager(int c) 
        {
            server_counter = c;

            Manager.UpdateCounter(c);
        }

        #endregion

        #region Successful mouseclick message

        [Command]
        public void CmdSendMouseClick(string p_name, UFOPlayer p_num, Vector3 m_pos) 
        {
            //LogMouseClick(p_name, p_num, m_pos);
            RegisterMouseClick(p_name, p_num, m_pos);
        }

        [Command]
        public void CmdSwitchTurn() 
        {
            SwitchTurn();
        }

        public void HandleUpdateMouseClick(Vector3 oldValue, Vector3 newValue) => UpdateMouseClick();

        [Client]
        public void UpdateMouseClick() 
        {
            
        }

        [Server]
        public void LogMouseClick(string p_name, UFOPlayer p_num, Vector3 m_pos) 
        {
            GameObject obj_select = MakeRaycast(m_pos);
            string obj_tag = "null";

            if (obj_select != null) 
            {
                obj_tag = obj_select.tag;
            }

            Debug.Log("Click received from: " + p_name + " who is: " + p_num + " click pos: " + m_pos + " selection: " + obj_tag);
        }

        #endregion

        #region Server game logic (SUCCESS!)

        //------------------------------------------------------------
        // SERVER
        //------------------------------------------------------------

        [Server]
        public void RegisterMouseClick(string p_name, UFOPlayer p_num, Vector3 m_pos)
        {
            if (!ValidPlayerClick(p_num)) { return; }

            //Debug.Log("Click from " + p_name + " is valid");

            selection = MakeRaycast(m_pos);

            switch (selection_state) 
            {
                case SelectionState.NO_SELECTION:
                    DoNoSelectionSequence(p_num);
                    break;
                case SelectionState.UFO_SELECTION:
                    DoUFOSelectionSequence(p_name, p_num, m_pos);
                    break;
                default:
                    Debug.Log("STATE SELECTION ERROR");
                    break;
            }
        }
            
        [Server]
        public bool ValidPlayerClick(UFOPlayer p_num) 
        {
            if (connections < 2) { return false; }
            if (turn_tracker == p_num) { return true; }
            return false;
        }

        [Server]
        public GameObject MakeRaycast(Vector3 m_pos) 
        {
            RaycastHit2D raycast_hit = Physics2D.Raycast(m_pos, Vector3.forward, 100f);

            if (raycast_hit.collider != null)
            {
                return raycast_hit.collider.transform.parent.gameObject;
            }

            return null;
        }

        [Server]
        public void SwitchTurn() 
        {
            if (turn_tracker == UFOPlayer.P1)
            { 
                turn_tracker = UFOPlayer.P2;
            }
            else
            {
                turn_tracker = UFOPlayer.P1;
            }

            RpcUpdateTurnTracker(turn_tracker);

            if(turn_tracker == UFOPlayer.P1)
            {
                RpcClientUIUpdates(Manager.p1_name_text.text);
            }
            else
            {
                RpcClientUIUpdates(Manager.p2_name_text.text);
            }

            CheckForAvailableAttacks(turn_tracker);
            CheckForStalemate();
            if (just_queens_left) 
            {
                UpdateStalemateCounter();
            }
        }

        [Server]
        public void SetSelectionState(SelectionState s_state) 
        {
            selection_state = s_state;

            RpcUpdateSelectionState(selection_state);
        }

        [Server]
        public void DoNoSelectionSequence(UFOPlayer p_num) 
        {
            if (selection == null) { return; }
            if (selection.GetComponent<UFO_MP_2>() == null) { return; }
            if (selection.GetComponent<UFO_MP_2>().ufo_player != p_num) { return; }

            selected_tiles.Clear();
            UFOSelected(true);
            DoTileSelection();
            DoTileHighlight();
            selection_state = SelectionState.UFO_SELECTION;
            prev_selection = selection;
        }

        [Server]
        public void UFOSelected(bool b) 
        {
            if (b)
            {
                selection.GetComponent<UFO_MP_2>().SelectUFO();
                return;
            }

            //if (selection.GetComponent<UFO_MP_2>() != null) { selection.GetComponent<UFO_MP_2>().DeSelectUFO(); }
            if (prev_selection.GetComponent<UFO_MP_2>() != null) { prev_selection.GetComponent<UFO_MP_2>().DeSelectUFO(); }
        }

        [Server]
        public void DoTileSelection() 
        {
            Vector3 checker_pos = selection.transform.position + new Vector3(0f, 8f, 0f);

            switch (selection.GetComponent<UFO_MP_2>().ufo_state) 
            {
                case UFOState.DEFAULT:
                    DoDefaultSelection(checker_pos);
                    break;
                case UFOState.QUEEN:
                    DoQueenSelection(checker_pos);
                    break;
            }
        }

        [Server]
        public void DoDefaultSelection(Vector3 checker_pos) 
        {
            Vector3[] check_dirs = new Vector3[2];
            int checks_count = 2;

            switch (selection.GetComponent<UFO_MP_2>().ufo_player) 
            {
                case UFOPlayer.P1:
                    check_dirs[0] = up_dir;
                    check_dirs[1] = left_dir;
                    break;
                case UFOPlayer.P2:
                    check_dirs[0] = down_dir;
                    check_dirs[1] = right_dir;
                    break;
            }

            SelectTiles(checker_pos, check_dirs, checks_count);
        }

        [Server]
        public void DoQueenSelection(Vector3 checker_pos)
        {
            Vector3[] check_dirs = new Vector3[4];
            int checks_count = 7;

            check_dirs[0] = up_dir;
            check_dirs[1] = down_dir;
            check_dirs[2] = left_dir;
            check_dirs[3] = right_dir;

            SelectTiles(checker_pos, check_dirs, checks_count);
        }

        [Server]
        public void SelectTiles(Vector3 checker_pos, Vector3[] check_dirs, int checks_count)
        {
            bool dir_stop;

            for (int i = 0; i < check_dirs.GetLength(0); i++)
            {
                dir_stop = false;

                for (int j = 1; j <= checks_count; j++)
                {
                    Vector3 check_vector = check_dirs[i] * j;
                    GameObject check_object = MakeRaycast(checker_pos + check_vector);

                    if (check_object == null) { break; }

                    if (check_object.GetComponent<UFO_MP_2>() != null) 
                    {
                        if(check_object.GetComponent<UFO_MP_2>().ufo_player == selection.GetComponent<UFO_MP_2>().ufo_player) { break; }

                        if (dir_stop) { break; }
                        dir_stop = true;
                        continue;
                    }

                    if (check_object.GetComponent<PlayableTile_MP_2>() != null)
                    {
                        selected_tiles.Add(check_object);

                        if (selection.GetComponent<UFO_MP_2>().ufo_state == UFOState.DEFAULT) { break; }
                    }

                    if (dir_stop) { break; }
                }
            }
        }

        [Server]
        public int SelectTiles(Vector3 checker_pos, Vector3[] check_dirs, int checks_count, UFOPlayer p_num)
        {
            bool dir_stop;
            int availableCount = 0;
            UFOState ufo_state = MakeRaycast(checker_pos).GetComponent<UFO_MP_2>().ufo_state;

            for (int i = 0; i < check_dirs.GetLength(0); i++)
            {
                dir_stop = false;

                for (int j = 1; j <= checks_count; j++)
                {
                    Vector3 check_vector = check_dirs[i] * j;
                    GameObject check_object = MakeRaycast(checker_pos + check_vector);

                    if (check_object == null) { break; }

                    if (check_object.GetComponent<UFO_MP_2>() != null)
                    {
                        if (check_object.GetComponent<UFO_MP_2>().ufo_player == p_num) { break; }

                        if (dir_stop) { break; }
                        dir_stop = true;
                        continue;
                    }

                    if (check_object.GetComponent<PlayableTile_MP_2>() != null)
                    {
                        availableCount++;

                        if (ufo_state == UFOState.DEFAULT) { break; }
                    }

                    if (dir_stop) { break; }
                }
            }

            return availableCount;
        }

        [Server]
        public void DoTileHighlight()
        {
            if (selected_tiles.Count < 1) { return; }

            RpcSetTileHighlight(selected_tiles, selection.GetComponent<UFO_MP_2>().my_color, selection.GetComponent<UFO_MP_2>().ufo_player, true);
        }

        [Server]
        public void DoUFOSelectionSequence(string p_name, UFOPlayer p_num, Vector3 m_pos)
        {
            if (selection == null)
            {
                DoDeselectionSequence();
                return;
            }
            if (selection.GetComponent<PlayableTile_MP_2>() != null)
            {
                if (!ValidTile()) 
                {
                    return;
                }

                DoMoveSequence(p_num);
            }
            if (selection.GetComponent<UFO_MP_2>() != null) 
            {
                if (selection.GetComponent<UFO_MP_2>().ufo_player != p_num) { return; }
                if (selection.transform.position == prev_selection.transform.position)
                {
                    DoDeselectionSequence();
                    return;
                }

                DoDeselectionSequence();
                DoNoSelectionSequence(p_num);
            }
        }

        [Server]
        public bool ValidTile() 
        {
            bool res = false;

            if (selected_tiles.Count > 0) 
            {
                foreach(var tile in selected_tiles) 
                {
                    if (selection.transform.position == tile.transform.position) { res = true; }
                }
            }

            return res;
        }

        [Server]
        public void DoMoveSequence(UFOPlayer p_num)
        {
            DoDeselectionSequence();

            if (AttackAvailable(p_num))
            {
                DoUFOAttack(ufo_to_attack, p_num);
                ufo_to_attack = null;
            }

            prev_selection.GetComponent<UFO_MP_2>().MoveUFO(selection.transform.position);

            CheckForQueenChange(p_num);

            SwitchTurn();
        }

        [Server]
        public bool AttackAvailable(UFOPlayer p_num) 
        {
            bool res = false;
            Vector3 dir = Vector3.zero;
            Vector3 check_pos = prev_selection.transform.position + new Vector3(0f, 8f, 0f);

            if(selection.transform.position.y > prev_selection.transform.position.y) 
            {
                dir = up_dir;
            }
            else if (selection.transform.position.y < prev_selection.transform.position.y)
            {
                dir = down_dir;
            }
            else if (selection.transform.position.x > prev_selection.transform.position.x)
            {
                dir = right_dir;
            }
            else if (selection.transform.position.x < prev_selection.transform.position.x)
            {
                dir = left_dir;
            }

            for (int i = 1; i < 10; i++)
            {
                GameObject check_obj = MakeRaycast(check_pos + (dir * i));

                if (check_obj == null) { break; }
                if (check_obj.GetComponent<UFO_MP_2>() != null)
                {
                    if (check_obj.GetComponent<UFO_MP_2>().ufo_player != p_num) 
                    {
                        if (dir == up_dir) 
                        {
                            if (check_obj.transform.position.y > selection.transform.position.y) 
                            {
                                break;
                            }
                        }
                        if (dir == down_dir)
                        {
                            if (check_obj.transform.position.y < selection.transform.position.y)
                            {
                                break;
                            }
                        }
                        if (dir == right_dir)
                        {
                            if (check_obj.transform.position.x > selection.transform.position.x)
                            {
                                break;
                            }
                        }
                        if (dir == left_dir)
                        {
                            if (check_obj.transform.position.x < selection.transform.position.x)
                            {
                                break;
                            }
                        }

                        ufo_to_attack = check_obj;
                        res = true;
                        break;
                    }
                }
            }

            return res;
        }

        [Server]
        public void DoUFOAttack(GameObject target, UFOPlayer p_num) 
        {
            RpcSetUFODeathAnimation(target, p_num);

            if (p_num == UFOPlayer.P1)
            {
                p_1_counter++;
                ServerUpdateCounter(p_1_counter, p_num);
                if (p_1_counter >= 12)
                {
                    RpcEndGame(Manager.p1_name_text.text, "wins!");
                }
            }
            else
            {
                p_2_counter++;
                ServerUpdateCounter(p_2_counter, p_num);
                if (p_2_counter >= 12)
                {
                    RpcEndGame(Manager.p2_name_text.text, "wins!");
                }
            }
        }

        [Server]
        public void CheckForQueenChange(UFOPlayer p_num) 
        {
            if (p_num == UFOPlayer.P1) 
            {
                foreach (var pos in P1_queen_tiles) 
                {
                    if ((Vector2)selection.transform.position == pos) 
                    {
                        RpcUpdateQueenChange(p_num, prev_selection);
                    }
                }
            }

            if (p_num == UFOPlayer.P2)
            {
                foreach (var pos in P2_queen_tiles)
                {
                    if ((Vector2)selection.transform.position == pos)
                    {
                        RpcUpdateQueenChange(p_num, prev_selection);
                    }
                }
            }
        }

        [Server]
        public void DoDeselectionSequence() 
        {
            RpcSetTileHighlight(selected_tiles, prev_selection.GetComponent<UFO_MP_2>().my_color, prev_selection.GetComponent<UFO_MP_2>().ufo_player, false);
            UFOSelected(false);
            selected_tiles.Clear();
            selection_state = SelectionState.NO_SELECTION;
        }

        [Server]
        public void CheckForAvailableAttacks(UFOPlayer p_num) 
        {
            StartCoroutine(CheckAttacksCoroutine(p_num));
        }

        [Server]
        public IEnumerator CheckAttacksCoroutine(UFOPlayer p_num) 
        {
            yield return new WaitForSeconds(0f);

            int available_tiles = 0;

            if (p_num == UFOPlayer.P1) 
            {
                foreach(var ufo in Manager.p1_UFOs) 
                {
                    if (ufo.activeSelf)
                    {
                        if (ufo.GetComponent<UFO_MP_2>().hitbox.activeSelf)
                        {
                            if (ufo.GetComponent<UFO_MP_2>().ufo_state == UFOState.DEFAULT)
                            {
                                available_tiles += SelectTiles(new Vector3(ufo.GetComponent<UFO_MP_2>().hitbox.transform.position.x, ufo.GetComponent<UFO_MP_2>().hitbox.transform.position.y + 8f, 0f), new Vector3[] { up_dir, left_dir }, 2, p_num);
                            }
                            else
                            {
                                available_tiles += SelectTiles(new Vector3(ufo.GetComponent<UFO_MP_2>().hitbox.transform.position.x, ufo.GetComponent<UFO_MP_2>().hitbox.transform.position.y + 8f, 0f), new Vector3[] { up_dir, down_dir, left_dir, right_dir }, 7, p_num);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var ufo in Manager.p2_UFOs)
                {
                    if (ufo.activeSelf)
                    {
                        if (ufo.GetComponent<UFO_MP_2>().hitbox.activeSelf)
                        {
                            if (ufo.GetComponent<UFO_MP_2>().ufo_state == UFOState.DEFAULT)
                            {
                                available_tiles += SelectTiles(new Vector3(ufo.GetComponent<UFO_MP_2>().hitbox.transform.position.x, ufo.GetComponent<UFO_MP_2>().hitbox.transform.position.y + 8f, 0f), new Vector3[] { down_dir, right_dir }, 2, p_num);
                            }
                            else
                            {
                                available_tiles += SelectTiles(new Vector3(ufo.GetComponent<UFO_MP_2>().hitbox.transform.position.x, ufo.GetComponent<UFO_MP_2>().hitbox.transform.position.y + 8f, 0f), new Vector3[] { up_dir, down_dir, left_dir, right_dir }, 7, p_num);
                            }
                        }
                    }
                }
            }

            //Debug.Log("available_tiles for " + p_num + ": " + available_tiles);
            if (available_tiles < 1) 
            {
                yield return new WaitForSeconds(2f);

                if (p_num == UFOPlayer.P1)
                {
                    RpcEndGame(Manager.p2_name_text.text + " wins!", "No moves for " + Manager.p1_name_text.text);
                }
                else
                {
                    RpcEndGame(Manager.p1_name_text.text + " wins!", "No moves for " + Manager.p2_name_text.text);
                }
            }

            yield break;
        }

        [Server]
        public void CheckForStalemate() 
        {
            bool ufo_1_left_p1 = false;
            bool ufo_1_left_p2 = false;

            int ufo_p1_count = 0;
            int ufo_p2_count = 0;

            UFO_MP_2 p1_last_ufo = null;
            UFO_MP_2 p2_last_ufo = null;

            bool ufo_1_left_p1_queen = false;
            bool ufo_1_left_p2_queen = false;

            for (int i = 0; i < Manager.p1_UFOs.GetLength(0); i++) 
            {
                if (Manager.p1_UFOs[i].GetComponent<UFO_MP_2>().hitbox.activeSelf || Manager.p1_UFOs[i].activeSelf) { ufo_p1_count++; p1_last_ufo = Manager.p1_UFOs[i].GetComponent<UFO_MP_2>(); }
                if (Manager.p2_UFOs[i].GetComponent<UFO_MP_2>().hitbox.activeSelf || Manager.p2_UFOs[i].activeSelf) { ufo_p2_count++; p2_last_ufo = Manager.p2_UFOs[i].GetComponent<UFO_MP_2>(); }
            }

            if (ufo_p1_count == 1) { ufo_1_left_p1 = true; }
            if (ufo_p2_count == 1) { ufo_1_left_p2 = true; }

            if (ufo_1_left_p1) 
            {
                if (p1_last_ufo.ufo_state == UFOState.QUEEN) { ufo_1_left_p1_queen = true; }
            }
            if (ufo_1_left_p2)
            {
                if (p2_last_ufo.ufo_state == UFOState.QUEEN) { ufo_1_left_p2_queen = true; }
            }

            if (ufo_1_left_p1_queen && ufo_1_left_p2_queen) 
            {
                //Debug.Log("ONLY 1 QUEEN LEFT FOR EACH PLAYER");

                just_queens_left = true;
                RpcUpdateStalemateBool(just_queens_left);
            }

            //Debug.Log("p1 only 1 ufo left?: " + ufo_1_left_p1);
            //Debug.Log("is it a queen?: " + ufo_1_left_p1_queen);
            //Debug.Log("p2 only 1 ufo left?: " + ufo_1_left_p2);
            //Debug.Log("is it a queen?: " + ufo_1_left_p2_queen);
        }

        [Server]
        public void UpdateStalemateCounter() 
        {
            stalemate_counter--;
            RpcUpdateStalemateCounter(stalemate_counter);

            RpcClientStalemateCounterUpdate(stalemate_counter);

            if (stalemate_counter == 0) 
            {
                RpcEndGame("Draw!", "");
            }
        }

        //------------------------------------------------------------
        // CLIENT AND CLIENTRPCs
        //------------------------------------------------------------

        [ClientRpc]
        public void RpcUpdateStalemateCounter(int c)
        {
            UpdateStalemateCounter(c);
        }

        [Client]
        public void UpdateStalemateCounter(int c)
        {
            stalemate_counter = c;
        }

        [ClientRpc]
        public void RpcUpdateStalemateBool(bool b) 
        {
            UpdateStalemateBool(b);
        }

        [Client]
        public void UpdateStalemateBool(bool b) 
        {
            just_queens_left = b;
        }

        [ClientRpc]
        public void RpcClientStalemateCounterUpdate(int c) 
        {
            ClientStalemateCounterUpdate(c);
        }

        [Client]
        public void ClientStalemateCounterUpdate(int c) 
        {
            Manager.stalemate_obj.SetActive(true);
            Manager.stalemate_text.text = c.ToString();
        }

        [ClientRpc]
        public void RpcUpdateQueenChange(UFOPlayer p_num, GameObject obj) 
        {
            UpdateQueenChange(p_num, obj);
        }

        [Client]
        public void UpdateQueenChange(UFOPlayer p_num, GameObject obj) 
        {
            if (p_num == UFOPlayer.P1)
            {
                foreach (var ufo in Manager.p1_UFOs) 
                {
                    if (obj.name == ufo.name)
                    {
                        StartCoroutine(MakeUFOQueen(ufo));
                    }
                }
            }
            if (p_num == UFOPlayer.P2)
            {
                foreach (var ufo in Manager.p2_UFOs)
                {
                    if (obj.name == ufo.name)
                    {
                        StartCoroutine(MakeUFOQueen(ufo));
                    }
                }
            }
        }

        [Client]
        public IEnumerator MakeUFOQueen(GameObject ufo) 
        {
            yield return new WaitForSeconds(0.25f);

            ufo.GetComponent<UFO_MP_2>().MakeQueen();

            yield break;
        }

        [ClientRpc]
        public void RpcEndGame(string n, string text1) 
        {
            Manager.GameOverMenu(n, text1);
        }

        [ClientRpc]
        public void RpcUpdateTurnTracker(UFOPlayer t_track) 
        {
            UpdateTurnTracker(t_track);
        }

        [Client]
        public void UpdateTurnTracker(UFOPlayer t_track) 
        {
            turn_tracker = t_track;
        }

        [ClientRpc]
        public void RpcUpdateSelectionState(SelectionState s_state) 
        {
            UpdateSelectionState(s_state);
        }

        [Client]
        public void UpdateSelectionState(SelectionState s_state) 
        {
            selection_state = s_state;
        }

        [ClientRpc]
        public void RpcSetTileHighlight(List<GameObject> tiles, Color p_color, UFOPlayer p_num, bool select_bool) 
        {
            SetTileHighlight(tiles, p_color, p_num, select_bool);
        }

        [Client]
        public void SetTileHighlight(List<GameObject> tiles, Color p_color, UFOPlayer p_num, bool select_bool) 
        {
            if (!isLocalPlayer) { return; }
            if (playerNum != p_num) { return; }

            foreach(var sel_tile in tiles) 
            {
                foreach(var tile in Manager.playable_tiles) 
                {
                    if (sel_tile.name == tile.name) 
                    {
                        if (select_bool)
                        {
                            tile.GetComponent<PlayableTile_MP_2>().SelectTile(p_color);
                        }
                        else
                        {
                            tile.GetComponent<PlayableTile_MP_2>().DeSelectTile();
                        }
                    }
                }
            }
        }

        [ClientRpc]
        public void RpcSetUFODeathAnimation(GameObject target, UFOPlayer p_num) 
        {
            SetUFODeathAnimation(target, p_num);
        }

        [Client]
        public void SetUFODeathAnimation(GameObject target, UFOPlayer p_num) 
        {
            if (p_num == UFOPlayer.P1) 
            {
                foreach (var ufo in Manager.p2_UFOs) 
                {
                    if (target.name == ufo.name) 
                    {
                        ufo.GetComponent<UFO_MP_2>().UFODeath();
                    }
                }

                return;
            }

            foreach (var ufo in Manager.p1_UFOs)
            {
                if (target.name == ufo.name)
                {
                    ufo.GetComponent<UFO_MP_2>().UFODeath();
                }
            }
        }

        #endregion

        #region UI methods (NOTHING WORKS!)

        //------------------------------------------------------------
        // SERVER
        //------------------------------------------------------------

        [Command]
        public void CmdSendData()
        {
            AcceptData(displayName, playerNum);
        }

        [Server]
        public void AcceptData(string p_name, UFOPlayer p_num) 
        {
            player_names.Add(p_name);
            player_nums.Add(p_num);

            RpcUpdateClientUI(player_names, player_nums);
        }

        [Server]
        public void OnGameStartedMethod(object sender, EventArgs e) 
        {
            RpcSendData();
        }

        //------------------------------------------------------------
        // CLIENT
        //------------------------------------------------------------

        [ClientRpc]
        public void RpcSendData() 
        {
            SendData();
        }

        [Client]
        public void SendData() 
        {
            if (!isLocalPlayer) { return; }

            CmdSendData();
        }

        [ClientRpc]
        public void RpcUpdateClientUI(List<string> list_names, List<UFOPlayer> list_nums) 
        {
            UpdateClientUI(list_names, list_nums);
        }

        [Client]
        public void UpdateClientUI(List<string> list_names, List<UFOPlayer> list_nums) 
        {
            for (int i = 0; i < list_names.Count; i++) 
            {
                if (list_nums[i] == UFOPlayer.P1) 
                {
                    Manager.p1_name_text.text = list_names[i];
                }
                else 
                {
                    Manager.p2_name_text.text = list_names[i];
                }
            }
        }

        #endregion

        #region UI methods 2

        [Command]
        public void CmdSendReadyToken() 
        {
            AcceptReadyToken();
        }

        [Server]
        public void AcceptReadyToken() 
        {
            connections++;

            if (connections > 2) { connections = 1; }

            RpcUpdateConnectionsCount(connections);

            Debug.Log("connections: " + connections);
        }

        [ClientRpc]
        public void RpcUpdateConnectionsCount(int c) 
        {
            UpdateConnectionsCount(c);
        }

        [Client]
        public void UpdateConnectionsCount(int c) 
        {
            connections = c;
        }

        [ClientRpc]
        public void RpcClientUIUpdates(string turn) 
        {
            ClientUIUpdates(turn);
        }

        [Client]
        public void ClientUIUpdates(string turn) 
        {
            Manager.SwitchArrows();

            Manager.PopupTurnSwitch(turn);
        }

        [Client]
        public void CQuitMatch() 
        {
            Manager.QuitMatch();
        }

        #endregion
    }
}
