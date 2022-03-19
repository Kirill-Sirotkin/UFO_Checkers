using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UFOCheckers
{
    public class Crown : MonoBehaviour
    {
        [field: SerializeField] private GameObject sprite_obj;

        void LateUpdate()
        {
            transform.position = new Vector3(sprite_obj.transform.position.x, transform.position.y, transform.position.z);
        }
    }
}
