using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace UFOCheckers
{
    public class PlayableTile_MP_2 : NetworkBehaviour
    {
        [field: SerializeField] private SpriteRenderer sprite_rend;
        [field: SerializeField] private Animator selection_anim;

        public void SetColor(Color color) 
        {
            sprite_rend.color = color;
        }
        public void SelectTile(Color color)
        {
            SetColor(color);
            selection_anim.SetFloat("Speed", 1.75f);
            selection_anim.SetTrigger("Select");
        }
        public void DeSelectTile()
        {
            selection_anim.SetTrigger("Deselect");
        }
    }
}
