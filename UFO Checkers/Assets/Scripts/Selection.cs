using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UFOCheckers
{
    public class Selection : MonoBehaviour
    {
        [field: SerializeField] private GameObject selection_obj;
        [field: SerializeField] private Animator animator;
        [field: SerializeField] private float speed;
        [field: SerializeField] private SpriteRenderer sprite_rend;

        public bool isSelected { get; private set; } = false;

        private void Awake()
        {
            animator.SetFloat("Speed", speed);
        }

        public void DoTileSelection(Color color) 
        {
            ChangeColor(color);
            //animator.SetTrigger("SelectionTrigger");
            isSelected = true;
            //selection_obj.SetActive(true);
        }
        public void DoTileDeselection()
        {
            //selection_obj.SetActive(false);
            //animator.SetTrigger("DeselectionTrigger");
            ChangeColor(Color.clear);
            isSelected = false;
        }

        public void ChangeColor(Color color) 
        {
            sprite_rend.color = color;
        }
    }
}
