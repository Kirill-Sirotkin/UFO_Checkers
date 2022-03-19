using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace UFOCheckers
{
    public class UFO_MP_2 : NetworkBehaviour
    {
        [field: SerializeField] private Animator animator;
        [field: SerializeField] private GameObject crown;
        [field: SerializeField] private Animator explosion_animator;
        [field: SerializeField] public GameObject hitbox { get; set; }

        [field: SerializeField] public UFOState ufo_state { get; private set; } = UFOState.DEFAULT;
        [field: SerializeField] public UFOPlayer ufo_player { get; private set; }
        [field: SerializeField] public Color my_color { get; set; }

        private float lerp_speed = 0.05f;
        private float anim_speed;

        private Vector2[] P1_queen_tiles = new Vector2[] { new Vector2(-35.16f, 27.04f), new Vector2(-23.44f, 33.8f), new Vector2(-11.72f, 40.56f), new Vector2(0f, 47.32f) };
        private Vector2[] P2_queen_tiles = new Vector2[] { new Vector2(0f, 0f), new Vector2(11.72f, 6.76f), new Vector2(23.44f, 13.52f), new Vector2(35.16f, 20.28f) };

        private Vector2[] my_queen_tiles = new Vector2[0];

        private void Awake()
        {
            anim_speed = Random.Range(1.0f, 2.0f);
            animator.SetFloat("animSpeed", anim_speed);
            if (ufo_player == UFOPlayer.P1)
            {
                my_queen_tiles = P1_queen_tiles;
            }
            else
            {
                my_queen_tiles = P2_queen_tiles;
            }
        }
        private void OnEnable()
        {
            animator.SetFloat("animSpeed", anim_speed);
        }
        public override void OnStartAuthority()
        {
            animator.SetFloat("animSpeed", anim_speed);

            base.OnStartAuthority();
        }
        public void SelectUFO()
        {
            animator.SetBool("isSelected", true);
        }
        public void DeSelectUFO()
        {
            animator.SetBool("isSelected", false);
        }
        public void UFODeath() 
        {
            StartCoroutine(UFODeathCoroutine());
        }
        private IEnumerator UFODeathCoroutine()
        {
            animator.SetTrigger("isDead");
            hitbox.SetActive(false);
            crown.SetActive(false);
            explosion_animator.SetTrigger("Explode");

            yield return new WaitForSeconds(1f);

            gameObject.SetActive(false);
        }
        public void MoveUFO(Vector3 target_location) 
        {
            StartCoroutine(MoveUFOCoroutine(target_location));
        }
        private IEnumerator MoveUFOCoroutine(Vector3 target_location)
        {
            float distance_to_target = (transform.position - target_location).sqrMagnitude;
            hitbox.transform.position = new Vector3(target_location.x, target_location.y, -0.01f);

            while (distance_to_target > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, target_location, lerp_speed);
                hitbox.transform.position = new Vector3(target_location.x, target_location.y, -0.01f);

                if (Vector3.Distance(transform.position, target_location) < 0.01f)
                {
                    transform.position = target_location;
                    hitbox.transform.position = new Vector3(target_location.x, target_location.y, -0.01f);
                    distance_to_target = 0f;
                }

                yield return null;
            }

            //for (int i = 0; i < my_queen_tiles.GetLength(0); i++)
            //{
            //    if ((Vector2)transform.position == my_queen_tiles[i])
            //    {
            //        crown.SetActive(true);
            //        ufo_state = UFOState.QUEEN;
            //    }
            //}
        }
        public void MakeQueen()
        {
            crown.SetActive(true);
            ufo_state = UFOState.QUEEN;
        }
    }
}
