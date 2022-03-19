using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UFOCheckers
{
    [ExecuteInEditMode]
    public class Sprite_Layer_Adjustment : MonoBehaviour
    {
        [field: SerializeField] private SpriteRenderer sprite_renderer;

        private void Awake()
        {
            sprite_renderer.sortingOrder = Mathf.RoundToInt(transform.position.y) * -1;
        }

        private void OnEnable()
        {
            sprite_renderer.sortingOrder = Mathf.RoundToInt(transform.position.y) * -1;
        }

        void Update()
        {
            sprite_renderer.sortingOrder = Mathf.RoundToInt(transform.position.y) * -1;
        }
    }
}
