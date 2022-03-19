using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UFOCheckers
{
    public enum GameState { START, P1_TURN, P2_TURN, P1_WIN, P2_WIN }

    public class GameCoordinator : MonoBehaviour
    {
        [field: SerializeField] public GameState state { get; set; }

        [field: SerializeField] public Player Player1 { get; set; }
        [field: SerializeField] public Player Player2 { get; set; }

        [field: SerializeField] private TMP_Text p1_text;
        [field: SerializeField] private TMP_Text p2_text;

        [field: SerializeField] public bool turn_switch { get; set; }

        [field: SerializeField] private TMP_Text turn_switch_text;

        [field: SerializeField] private Animator turn_switch_animator;

        private int p1_score_counter;
        private int p2_score_counter;

        [field: SerializeField] private GameObject EndGameMenu;
        [field: SerializeField] private TMP_Text endGameText;

        [field: SerializeField] private GameObject p1_arrow;
        [field: SerializeField] private GameObject p2_arrow;

        private Coroutine player_controls_switch;

        private void Awake()
        {
            DoTurnSwitch();
            p1_score_counter = 0;
            p2_score_counter = 0;
        }

        public void DoTurnSwitch() 
        {
            if (turn_switch) 
            {
                if(player_controls_switch != null) 
                {
                    StopCoroutine(player_controls_switch);
                }

                Player1.enabled = false;

                player_controls_switch = StartCoroutine(SwitchPlayerControls(false, true));

                p1_arrow.SetActive(false);
                p2_arrow.SetActive(true);

                if (p1_score_counter!= 12 && p2_score_counter != 12)
                {
                    InitiateTurnSwitchNotification("player_2");
                }
            }
            if (!turn_switch)
            {
                if (player_controls_switch != null)
                {
                    StopCoroutine(player_controls_switch);
                }

                Player2.enabled = false;

                player_controls_switch = StartCoroutine(SwitchPlayerControls(true, false));

                p1_arrow.SetActive(true);
                p2_arrow.SetActive(false);

                if (p1_score_counter != 12 && p2_score_counter != 12)
                {
                    InitiateTurnSwitchNotification("player_1");
                }
            }

            turn_switch = !turn_switch;
        }

        public void UpdateScore(UFOPlayer player_num) 
        {
            if(player_num == UFOPlayer.P1)
            {
                p1_score_counter++;

                if(p1_score_counter == 12) 
                {
                    EndGame("player_1");
                }

                p1_text.text = p1_score_counter.ToString();
                print("P1 +1");
            }
            else
            {
                p2_score_counter++;

                if (p2_score_counter == 12)
                {
                    EndGame("player_2");
                }

                p2_text.text = p2_score_counter.ToString();
                print("P2 +1");
            }
        }

        private void InitiateTurnSwitchNotification(string players_name)
        {
            if (turn_switch_animator.GetCurrentAnimatorStateInfo(0).IsName("TurnSwitchText_Popup")) 
            {
                print("------ POPUP ACTIVE!!!! ------");
                turn_switch_animator.Play("TurnSwitchText_Idle");
            }
            turn_switch_text.text = players_name + "'s turn!";
            turn_switch_animator.SetTrigger("Popup");
        }

        private void EndGame(string player_name)
        {
            endGameText.text = "Game Over! " + player_name + " won!";
            EndGameMenu.SetActive(true);
        }

        private IEnumerator SwitchPlayerControls(bool p1, bool p2) 
        {
            yield return new WaitForSeconds(0f);

            Player1.enabled = p1;
            Player2.enabled = p2;

            yield break;
        }
    }
}
