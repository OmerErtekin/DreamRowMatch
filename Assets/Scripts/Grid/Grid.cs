using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform objectContainer;
    [SerializeField] private List<Sprite> typeSprites;
    private GridObjectTypes objectType;
    public GridPosition gridPosition;
    public GridPosition GridIndex => gridPosition;
    public GridObjectTypes ObjectType => objectType;
    #endregion
    #region Components
    private SpriteRenderer objectSprite;
    #endregion

    public void InitializeGrid(int i, int j, GridObjectTypes typeToCreate)
    {
        gridPosition.i = i;
        gridPosition.j = j;
        objectSprite = objectContainer.GetComponent<SpriteRenderer>();
        objectSprite.sprite = typeSprites[(int)typeToCreate];
        objectType = typeToCreate;
    }
}

[System.Serializable]
public struct GridPosition
{
    public int i; public int j;
}

public enum GridObjectTypes
{
    Red,
    Green,
    Blue,
    Yellow,
    Matched,
}
