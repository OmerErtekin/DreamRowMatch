using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform objectContainer;
    [SerializeField] private List<Sprite> typeSprites;
    public GridObjectTypes objectType;
    #endregion
    #region Components
    private SpriteRenderer objectSprite;
    #endregion

    public void InitializeGrid(GridObjectTypes typeToCreate)
    {
        objectSprite = objectContainer.GetComponent<SpriteRenderer>();
        objectSprite.sprite = typeSprites[(int)typeToCreate];
        objectType = typeToCreate;
    }
}


public enum GridObjectTypes
{
    Red,
    Green,
    Blue,
    Yellow,
    Matched,
}
