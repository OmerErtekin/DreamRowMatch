using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    [SerializeField] private List<GameObject> borders = new List<GameObject>();
    public void SetBorder(Direction direction)
    {
        borders[(int)direction].gameObject.SetActive(true);    
    }
}
