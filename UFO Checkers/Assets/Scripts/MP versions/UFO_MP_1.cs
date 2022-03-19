using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace UFOCheckers
{
    public class UFO_MP_1 : MonoBehaviour
    {
        [field: SerializeField] private Animator animator;
        [field: SerializeField] private GameObject crown;
        [field: SerializeField] private GameObject explosion;
        [field: SerializeField] private Animator explosion_anim;
        [field: SerializeField] private GameObject hitbox;

        [field: SerializeField] public UFOState ufo_state { get; private set; } = UFOState.DEFAULT;
        [field: SerializeField] public UFOPlayer ufo_player { get; private set; }
        [field: SerializeField] private Color my_color;

        private float lerp_speed = 0.05f;

        private Vector2[] P1_queen_tiles = new Vector2[] { new Vector2(-35.16f, 27.04f), new Vector2(-23.44f, 33.8f), new Vector2(-11.72f, 40.56f), new Vector2(0f, 47.32f) };
        private Vector2[] P2_queen_tiles = new Vector2[] { new Vector2(0f, 0f), new Vector2(11.72f, 6.76f), new Vector2(23.44f, 13.52f), new Vector2(35.16f, 20.28f) };

        private Vector2[] my_queen_tiles = new Vector2[0];

        private Vector2[] up_direction_tiles = new Vector2[] { new Vector2(0f, 6.76f), new Vector2(0f, 13.52f), new Vector2(0f, 20.28f), new Vector2(0f, 27.04f), new Vector2(0f, 33.8f), new Vector2(0f, 40.56f), new Vector2(0f, 47.32f) };
        private Vector2[] down_direction_tiles = new Vector2[] { new Vector2(0f, -6.76f), new Vector2(0f, -13.52f), new Vector2(0f, -20.28f), new Vector2(0f, -27.04f), new Vector2(0f, -33.8f), new Vector2(0f, -40.56f), new Vector2(0f, -47.32f) };
        private Vector2[] left_direction_tiles = new Vector2[] { new Vector2(-11.72f, 0f), new Vector2(-23.44f, 0f), new Vector2(-35.16f, 0f), new Vector2(-46.88f, 0f), new Vector2(-58.6f, 0f), new Vector2(-70.32f, 0f), new Vector2(-82.04f, 0f) };
        private Vector2[] right_direction_tiles = new Vector2[] { new Vector2(11.72f, 0f), new Vector2(23.44f, 0f), new Vector2(35.16f, 0f), new Vector2(46.88f, 0f), new Vector2(58.6f, 0f), new Vector2(70.32f, 0f), new Vector2(82.04f, 0f) };

        private List<Selection> selected_tiles = new List<Selection>();

        private void Awake()
        {
            animator.SetFloat("animSpeed", Random.Range(1.0f, 2.0f));
            if(ufo_player == UFOPlayer.P1) 
            {
                my_queen_tiles = P1_queen_tiles;
            }
            else 
            {
                my_queen_tiles = P2_queen_tiles;
            }
        }
        public void SelectUFO()
        {
            animator.SetBool("isSelected", true);
            CheckTileSelection();
        }
        public void DeSelectUFO()
        {
            animator.SetBool("isSelected", false);

            for (int i =0;i< selected_tiles.Count; i++) 
            {
                selected_tiles[i].DoTileDeselection();
            }

            selected_tiles.Clear();
        }
        public IEnumerator UFODeath() 
        {
            animator.SetTrigger("isDead");
            hitbox.SetActive(false);
            crown.SetActive(false);
            //explosion.SetActive(true);
            explosion_anim.SetTrigger("Explode");

            yield return new WaitForSeconds(1f);

            gameObject.SetActive(false);
        }
        public IEnumerator MoveUFO(Vector3 target_location) 
        {
            //hitbox.SetActive(false);
            float distance_to_target = (transform.position - target_location).sqrMagnitude;
            hitbox.transform.position = target_location;

            while (distance_to_target > 0.01f) 
            {
                transform.position = Vector3.Lerp(transform.position, target_location, lerp_speed);
                hitbox.transform.position = new Vector3(target_location.x, target_location.y, -0.01f);

                if ((Vector3.Distance(transform.position, target_location) < 5f) && hitbox.activeSelf == false)
                {
                    hitbox.SetActive(true);
                }
                if (Vector3.Distance(transform.position, target_location) < 0.01f) 
                {
                    transform.position = target_location;
                    hitbox.transform.position = new Vector3(target_location.x, target_location.y, -0.01f);
                    distance_to_target = 0f;
                }

                yield return null;
            }

            for (int i = 0; i < my_queen_tiles.GetLength(0); i++) 
            {
                if((Vector2)transform.position == my_queen_tiles[i]) 
                {
                    crown.SetActive(true);
                    ufo_state = UFOState.QUEEN;
                }
            }
            //print("HITBOX ENABLED AGAIN!");
        }
        private void CheckTileSelection() 
        {
            if (ufo_state == UFOState.DEFAULT) 
            {
                if (ufo_player == UFOPlayer.P1) 
                {
                    SelectInDirection(up_direction_tiles);
                    SelectInDirection(left_direction_tiles);

                    return;
                }

                if (ufo_player == UFOPlayer.P2)
                {
                    SelectInDirection(down_direction_tiles);
                    SelectInDirection(right_direction_tiles);

                    return;
                }
            }

            if (ufo_state == UFOState.QUEEN)
            {
                SelectInDirection(up_direction_tiles);
                SelectInDirection(left_direction_tiles);
                SelectInDirection(down_direction_tiles);
                SelectInDirection(right_direction_tiles);
            }
        }
        private void SelectInDirection(Vector2[] direction_vectors) 
        {
            bool initiate_stop = false;

            for (int i = 0; i < direction_vectors.GetLength(0); i++) 
            {
                Vector3 ray_direction = new Vector3(transform.position.x + direction_vectors[i].x, transform.position.y + direction_vectors[i].y + 8f, -10f);

                RaycastHit2D raycast_hit = Physics2D.Raycast(ray_direction, Vector3.forward, 100f);

                if (raycast_hit.collider != null)
                {
                    if (raycast_hit.collider.transform.parent.gameObject.GetComponent<Selection>() != null)
                    {
                        if (!raycast_hit.collider.transform.parent.gameObject.GetComponent<Selection>().isSelected)
                        {
                            raycast_hit.collider.transform.parent.gameObject.GetComponent<Selection>().DoTileSelection(my_color);
                        }
                        else 
                        {
                            raycast_hit.collider.transform.parent.gameObject.GetComponent<Selection>().ChangeColor(my_color);
                        }
                        selected_tiles.Add(raycast_hit.collider.transform.parent.gameObject.GetComponent<Selection>());

                        if (ufo_state == UFOState.DEFAULT) 
                        {
                            break;
                        }

                        if (initiate_stop) 
                        {
                            break;
                        }
                    }

                    if (raycast_hit.collider.transform.parent.gameObject.GetComponent<UFO>() != null)
                    {
                        if (initiate_stop)
                        {
                            break;
                        }

                        if (raycast_hit.collider.transform.parent.gameObject.GetComponent<UFO>().ufo_player != ufo_player) 
                        {
                            initiate_stop = true;
                            continue;
                        }
                        else 
                        {
                            break;
                        }
                    }

                    if (initiate_stop)
                    {
                        break;
                    }
                }
            }
        }
    }
}
