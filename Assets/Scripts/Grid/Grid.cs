using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform objectContainer;
    [SerializeField] private List<Sprite> typeSprites;
    private GridObjectTypes objectType;
    public GridIndex gridIndex;
    public GridIndex CurrentGridIndex => gridIndex;
    public GridObjectTypes ObjectType => objectType;
    #endregion
    #region Components
    private SpriteRenderer objectSprite;
    #endregion

    public void InitializeGrid(int raw, int column, GridObjectTypes typeToCreate)
    {
        gridIndex.raw = raw;
        gridIndex.column = column;
        objectSprite = objectContainer.GetComponent<SpriteRenderer>();
        objectSprite.sprite = typeSprites[(int)typeToCreate];
        name = typeToCreate.ToString();
        objectType = typeToCreate;
    }

    public void SwipeTheGrid(GridIndex newGridIndex,Vector3 targetPosition)
    {
        gridIndex = newGridIndex;
        transform.DOMove(targetPosition, 0.5f).SetTarget(this).SetEase(Ease.OutCubic);
    }
}

[System.Serializable]
public struct GridIndex
{
    public int raw; public int column;
}

public enum GridObjectTypes
{
    Red,
    Green,
    Blue,
    Yellow,
    Matched,
}
