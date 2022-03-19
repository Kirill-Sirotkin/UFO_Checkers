using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UFOCheckers
{
    public class UFO_MainMenu : MonoBehaviour
    {
        [field: SerializeField] private SpriteRenderer sprite_renderer;
        [field: SerializeField] private Animator animator;

        private void Awake()
        {
            AssignAnimation();
        }

        private void AssignAnimation() 
        {
            if (transform.localScale.x > 2f)
            {
                sprite_renderer.sortingOrder = 1;
                animator.SetFloat("Speed", 0.5f);
                animator.Play("UFO_MainMenu_Close");
                return;
            }
            if (transform.localScale.x > 1f)
            {
                sprite_renderer.sortingOrder = 1;
                animator.Play("UFO_MainMenu_Med");
                return;
            }
            if (transform.localScale.x > 0f)
            {
                sprite_renderer.sortingOrder = 1;
                animator.SetFloat("Speed", 0.5f);
                animator.Play("UFO_MainMenu_Far");
                return;
            }

            print("no assignment");
        }
    }
}
