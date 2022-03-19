using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UFOCheckers
{
    public class Player_Selection : MonoBehaviour
    {
        [field: SerializeField] private Camera main_camera;
        [field: SerializeField] private GameObject placeholder;

        private Vector3 mouse_position = -Vector3.one;

        private void Awake()
        {
            if(main_camera == null)
            {
                main_camera = Camera.main;
            }
        }

        private void Update()
        {
            mouse_position = main_camera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0f, 0f, -10f));
        }

        public GameObject SelectOnClick(LayerMask layerMask, UFOPlayer player_num)
        {
            RaycastHit2D raycast_hit = Physics2D.Raycast(mouse_position, Vector3.forward, 100f, layerMask);

            if (raycast_hit.collider != null)
            {
                if(raycast_hit.collider.transform.parent.gameObject.GetComponent<UFO>() != null) 
                {
                    if(raycast_hit.collider.transform.parent.gameObject.GetComponent<UFO>().ufo_player == player_num)
                    {
                        return raycast_hit.collider.transform.parent.gameObject;
                    }

                    return placeholder;
                }

                return raycast_hit.collider.transform.parent.gameObject;
            }

            //print("(!) MAKE SELECTION RETURNED NOTHING");
            return placeholder;
        }
    }
}
