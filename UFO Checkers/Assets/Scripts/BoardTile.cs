using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UFOCheckers
{
    public class BoardTile : MonoBehaviour
    {
        [field: SerializeField] GameObject selection;

        public void SelectTile() 
        {
            selection.SetActive(true);
        }
        public void DeSelectTile()
        {
            selection.SetActive(false);
        }
    }
}
