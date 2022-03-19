using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

namespace UFOCheckers
{
    public class Player_MP_1 : NetworkBehaviour
    {
        private SelectionState selection_state;
        private MoveDirection queen_move_direction;
        private Vector2 target_check_increment = new Vector2(0f, 0f);
        [field: SerializeField] public UFOPlayer player_num { get; private set; }
        public GameObject selection { get; set; } = null;
        public GameObject prev_selection { get; set; } = null;

        [field: SerializeField] public GameCoordinator coordinator { get; set; }

        [field: SerializeField] public LayerMask uFO_layer_mask { get; set; }
        [field: SerializeField] public LayerMask board_layer_mask { get; set; }

        [field: SerializeField] private Player_Selection_MP_1 player_selection;
        [field: SerializeField] private GameObject placeholder;

        private Coroutine moveUFO_coroutine;

        private Vector2[] defaultUFO_checkpoints = new Vector2[] { new Vector2(0f, 6.76f), new Vector2(-11.72f, 0f) };
        private Vector2[] attackUFO_checkpoints = new Vector2[] { new Vector2(0f, 13.52f), new Vector2(-23.44f, 0f) };
        private Vector2[] queenUFO_checkpoints = new Vector2[] { new Vector2(0f, 6.76f), new Vector2(0f, 13.52f), new Vector2(0f, 20.28f), new Vector2(0f, 27.04f), new Vector2(0f, 33.8f), new Vector2(0f, 40.56f), new Vector2(0f, 47.32f), new Vector2(0f, -6.76f), new Vector2(0f, -13.52f), new Vector2(0f, -20.28f), new Vector2(0f, -27.04f), new Vector2(0f, -33.8f), new Vector2(0f, -40.56f), new Vector2(0f, -47.32f), new Vector2(-11.72f, 0f), new Vector2(-23.44f, 0f), new Vector2(-35.16f, 0f), new Vector2(-46.88f, 0f), new Vector2(-58.6f, 0f), new Vector2(-70.32f, 0f), new Vector2(-82.04f, 0f), new Vector2(11.72f, 0f), new Vector2(23.44f, 0f), new Vector2(35.16f, 0f), new Vector2(46.88f, 0f), new Vector2(58.6f, 0f), new Vector2(70.32f, 0f), new Vector2(82.04f, 0f) };

        private void Awake()
        {
            selection = placeholder;
            prev_selection = placeholder;

            selection_state = SelectionState.NO_SELECTION;

            if (player_num == UFOPlayer.P2) 
            {
                for (int i = 0; i < defaultUFO_checkpoints.GetLength(0); i++) 
                {
                    defaultUFO_checkpoints[i] = defaultUFO_checkpoints[i] * -1;
                }
                for (int i = 0; i < attackUFO_checkpoints.GetLength(0); i++)
                {
                    attackUFO_checkpoints[i] = attackUFO_checkpoints[i] * -1;
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                //if (!EventSystem.current.IsPointerOverGameObject())
                //{
                //    prev_selection = selection;
                //    MakeSelection();
                //    DoUFOAnimation();
                //    UpdateSelectionState();
                //    OnMoveSelection();
                //}
                prev_selection = selection;
                MakeSelection();
                DoUFOAnimation();
                UpdateSelectionState();
                OnMoveSelection();
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                //print("----------------------");
                //print("Current: " + selection.name);
                //print("Previous: " + prev_selection.name);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                //print("State: " + selection_state);
            }
        }

        private void OnMoveSelection()
        {
            if (selection_state == SelectionState.MOVE_SELECTION) 
            {
                if(prev_selection.GetComponent<UFO>().ufo_state == UFOState.DEFAULT)
                {
                    if (CheckIfUFOMovementAvailable() || CheckIfUFOAttackAvailable())
                    {
                        DoUFOMovement();
                        if(coordinator != null) 
                        {
                            coordinator.DoTurnSwitch();
                        }
                    }
                }

                if (prev_selection.GetComponent<UFO>().ufo_state == UFOState.QUEEN) 
                {
                    if (CheckIfQueenMovementAvailable())
                    {
                        CheckQueenMoveDirection();
                        //print(queen_move_direction);
                        SetTargetCheckIncrement();
                        CheckQueenAttack();
                    }
                }

                selection_state = SelectionState.NO_SELECTION;
            }
        }

        private void DoUFOMovement()
        {
            if (moveUFO_coroutine != null)
            {
                StopCoroutine(moveUFO_coroutine);
            }

            moveUFO_coroutine = StartCoroutine(prev_selection.GetComponent<UFO>().MoveUFO(selection.transform.position));
        }

        private void MakeSelection()
        {
            switch (selection_state) 
            {
                case SelectionState.NO_SELECTION:
                    selection = player_selection.SelectOnClick(uFO_layer_mask, player_num);
                    break;
                case SelectionState.UFO_SELECTION:
                    selection = player_selection.SelectOnClick(board_layer_mask, player_num);
                    break;
                case SelectionState.MOVE_SELECTION:
                    selection = player_selection.SelectOnClick(board_layer_mask, player_num);
                    break;
            }
        }

        private void DoUFOAnimation()
        {
            if (prev_selection.GetComponent<UFO>() != null)
            {
                prev_selection.GetComponent<UFO>().DeSelectUFO();
            }
            if (selection.GetComponent<UFO>() != null) 
            {
                selection.GetComponent<UFO>().SelectUFO();
            }
            if((prev_selection.GetComponent<UFO>() != null) && (selection.GetComponent<UFO>() != null))
            {
                if ((prev_selection.name == selection.name) && (prev_selection.GetComponent<UFO>().ufo_player == selection.GetComponent<UFO>().ufo_player))
                {
                    prev_selection.GetComponent<UFO>().DeSelectUFO();
                }
            }
        }

        private void UpdateSelectionState() 
        {
            if (selection.name == prev_selection.name) 
            {
                selection_state = SelectionState.NO_SELECTION;
                selection = placeholder;
                //print("0");
                return;
            }
            if (selection.GetComponent<UFO>() != null && prev_selection.GetComponent<UFO>() != null)
            {
                selection_state = SelectionState.UFO_SELECTION;
                //print("1");
                return;
            }
            if (selection.GetComponent<UFO>() == null && prev_selection.GetComponent<UFO>() != null)
            {
                selection_state = SelectionState.MOVE_SELECTION;
                //print("2");
                return;
            }
            if (selection.GetComponent<UFO>() == null && prev_selection.GetComponent<UFO>() == null)
            {
                selection_state = SelectionState.NO_SELECTION;
                //print("3");
                return;
            }
            if (selection.GetComponent<UFO>() != null)
            {
                selection_state = SelectionState.UFO_SELECTION;
                //print("4");
                return;
            }
        }

        private bool CheckIfUFOMovementAvailable()
        {
            bool isMovementAvailable = false;
            Vector2[] check_points = new Vector2[0];

            //print("started movement check");

            check_points = defaultUFO_checkpoints;

            for (int i = 0; i < check_points.GetLength(0); i++)
            {
                //print("checked: " + ((Vector2)transform.position + check_points[i]));
                if ((Vector2)selection.transform.position == ((Vector2)prev_selection.transform.position + check_points[i]))
                {
                    isMovementAvailable = true;
                    //print("MOVEMENT IS AVAILABLE YAYAAY");
                }
            }

            return isMovementAvailable;
        }

        private bool CheckIfUFOAttackAvailable()
        {
            bool isAttackAvailable = false;
            Vector2 available_movement_coord = new Vector2(-1000f, -1000f);
            Vector2[] check_points = new Vector2[0];

            check_points = attackUFO_checkpoints;

            for (int i = 0; i < check_points.GetLength(0); i++)
            {
                //print("checked: " + ((Vector2)transform.position + check_points[i]));
                if ((Vector2)selection.transform.position == ((Vector2)prev_selection.transform.position + check_points[i]))
                {
                    isAttackAvailable = true;
                    available_movement_coord = selection.transform.position;
                    //print("MOVEMENT IS AVAILABLE YOYOYOYO");
                }
            }

            if(available_movement_coord != new Vector2(-1000f, -1000f)) 
            {
                isAttackAvailable = CheckAttackTarget();
            }

            return isAttackAvailable;
        }

        private bool CheckAttackTarget() 
        {
            Vector2 check_vector = new Vector2(selection.transform.position.x - prev_selection.transform.position.x, selection.transform.position.y - prev_selection.transform.position.y);
            check_vector = check_vector * 0.5f + new Vector2(0f, 6f);
            check_vector = (Vector2)prev_selection.transform.position + check_vector;

            print("Check Vector: " + check_vector);

            RaycastHit2D raycast_hit = Physics2D.Raycast(new Vector3(check_vector.x, check_vector.y, -10f), Vector3.forward, 100f);

            if (raycast_hit.transform.parent.gameObject.GetComponent<UFO>() != null)
            {
                if (player_num == UFOPlayer.P1)
                {
                    if(raycast_hit.transform.parent.gameObject.GetComponent<UFO>().ufo_player == UFOPlayer.P2) 
                    {
                        print("attack success");
                        KillUFO(raycast_hit.transform.parent.gameObject);
                        if(coordinator != null)
                        {
                            coordinator.UpdateScore(player_num);
                        }
                        return true;
                    }
                }
                if (player_num == UFOPlayer.P2)
                {
                    if (raycast_hit.transform.parent.gameObject.GetComponent<UFO>().ufo_player == UFOPlayer.P1)
                    {
                        print("attack success");
                        KillUFO(raycast_hit.transform.parent.gameObject);
                        if (coordinator != null)
                        {
                            coordinator.UpdateScore(player_num);
                        }
                        return true;
                    }
                }
            }

            print("attack FAIL");
            return false;
        }

        private bool CheckIfQueenMovementAvailable()
        {
            bool isMovementAvailable = false;
            Vector2[] check_points = new Vector2[0];

            //print("started movement check");

            check_points = queenUFO_checkpoints;

            for (int i = 0; i < check_points.GetLength(0); i++)
            {
                //print("checked: " + ((Vector2)transform.position + check_points[i]));
                if ((Vector2)selection.transform.position == ((Vector2)prev_selection.transform.position + check_points[i]))
                {
                    isMovementAvailable = true;
                    //print("MOVEMENT IS AVAILABLE YAYAAY");
                }
            }

            return isMovementAvailable;
        }

        private void CheckQueenMoveDirection() 
        {
            if(Mathf.FloorToInt(selection.transform.position.x) == Mathf.FloorToInt(prev_selection.transform.position.x)) 
            {
                if(selection.transform.position.y > prev_selection.transform.position.y) 
                {
                    queen_move_direction = MoveDirection.POS_Y;
                }
                else
                {
                    queen_move_direction = MoveDirection.NEG_Y;
                }
            }
            else
            {
                if (selection.transform.position.x > prev_selection.transform.position.x)
                {
                    queen_move_direction = MoveDirection.POS_X;
                }
                else
                {
                    queen_move_direction = MoveDirection.NEG_X;
                }
            }
        }

        private void SetTargetCheckIncrement() 
        {
            switch (queen_move_direction) 
            {
                case MoveDirection.POS_X:
                    target_check_increment = new Vector2(11.72f, 0f);
                    break;
                case MoveDirection.POS_Y:
                    target_check_increment = new Vector2(0f, 6.76f);
                    break;
                case MoveDirection.NEG_X:
                    target_check_increment = new Vector2(-11.72f, 0f);
                    break;
                case MoveDirection.NEG_Y:
                    target_check_increment = new Vector2(0f, -6.76f);
                    break;
            }
        }

        private void CheckQueenAttack() 
        {
            Vector2 coords_diff = new Vector2(selection.transform.position.x - prev_selection.transform.position.x, selection.transform.position.y - prev_selection.transform.position.y);
            int max_checks;

            if (Mathf.RoundToInt(coords_diff.x) != 0) 
            {
                max_checks = Mathf.RoundToInt(coords_diff.x / target_check_increment.x) - 1;
                //print("0: coords diff / incr " + (coords_diff.x / target_check_increment.x));
            }
            else
            {
                max_checks = Mathf.RoundToInt(coords_diff.y / target_check_increment.y) - 1;
                //print("1: coords diff / incr " + (coords_diff.y / target_check_increment.y));
            }

            //print("coords diff: " + coords_diff);
            //print("coords diff / incr " + (coords_diff.x / target_check_increment.x) + " --- " + (coords_diff.y / target_check_increment.y));
            //print("max checks: " + max_checks);

            if (max_checks == 0) 
            {
                DoUFOMovement();
                if (coordinator != null)
                {
                    coordinator.DoTurnSwitch();
                }
                return;
            }

            Vector2 start_vector = new Vector2(prev_selection.transform.position.x, prev_selection.transform.position.y + 6f);

            for (int i = 0; i < max_checks; i++) 
            {
                start_vector = start_vector + target_check_increment;

                RaycastHit2D raycast_hit = Physics2D.Raycast(new Vector3(start_vector.x, start_vector.y, -10f), Vector3.forward, 100f);

                print("checking coords: " + start_vector);
                print("checked object: " + raycast_hit.transform.parent.gameObject.name);

                if (raycast_hit.transform.parent.gameObject.GetComponent<UFO>() != null) 
                {
                    print("reached stage 1");
                    if(player_num != raycast_hit.transform.parent.gameObject.GetComponent<UFO>().ufo_player)
                    {
                        print("reached stage 2");
                        Vector2 additional_vector = new Vector2(start_vector.x + target_check_increment.x, start_vector.y + target_check_increment.y);
                        RaycastHit2D raycast_hit_1 = Physics2D.Raycast(new Vector3(additional_vector.x, additional_vector.y, -10f), Vector3.forward, 100f);

                        if (raycast_hit_1.transform.parent.gameObject.GetComponent<UFO>() == null)
                        {
                            print("reached stage 3");
                            selection = raycast_hit_1.transform.parent.gameObject;
                            DoUFOMovement();
                            if (coordinator != null)
                            {
                                coordinator.UpdateScore(player_num);
                            }
                            if (coordinator != null)
                            {
                                coordinator.DoTurnSwitch();
                            }
                            KillUFO(raycast_hit.transform.parent.gameObject);
                            return;
                        }
                    }

                    return;
                }
            }

            DoUFOMovement();
            if (coordinator != null)
            {
                coordinator.DoTurnSwitch();
            }
        }

        public void PlayerNumSetter(UFOPlayer pnum)
        {
            player_num = pnum;
        }

        private void KillUFO(GameObject ufo_object) 
        {
            StartCoroutine(ufo_object.GetComponent<UFO>().UFODeath());
        }
    }
}
