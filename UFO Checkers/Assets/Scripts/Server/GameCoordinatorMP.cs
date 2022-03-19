using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UFOCheckers
{
    public class GameCoordinatorMP : GameCoordinator
    {
        [field: SerializeField] LayerMask p1_layer_mask;
        [field: SerializeField] LayerMask p2_layer_mask;
        [field: SerializeField] LayerMask common_layer_mask;

        public void CheckRegistration(Player p)
        {
            if (Player1 == null)
            {
                Player1 = p;
                Player1.PlayerNumSetter(UFOPlayer.P1);
                Player1.uFO_layer_mask = p1_layer_mask;
                Player1.board_layer_mask = common_layer_mask;
                print("regged p1");
                return;
            }
            if (Player2 == null)
            {
                Player2 = p;
                Player2.PlayerNumSetter(UFOPlayer.P2);
                Player2.uFO_layer_mask = p2_layer_mask;
                Player2.board_layer_mask = common_layer_mask;
                print("regged p2");
                return;
            }

            print("REGISTRATION: ALL PLAYERS FILLED");
        }
    }
}
