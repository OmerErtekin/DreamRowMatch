using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform objectContainer;
    [SerializeField] private List<Sprite> typeSprites;
    private GridObjectTypes objectType;
    private GridIndex gridIndex;
    #endregion
    #region Components
    private SpriteRenderer objectSprite;
    #endregion
    #region Properties
    public GridIndex CurrentGridIndex => gridIndex;
    public GridObjectTypes ObjectType => objectType;
    #endregion
    public void InitializeGrid(int raw, int column, GridObjectTypes typeToCreate)
    {
        gridIndex.raw = raw;
        gridIndex.column = column;
        objectType = typeToCreate;
        objectSprite = objectContainer.GetComponent<SpriteRenderer>();
        objectSprite.sprite = typeSprites[(int)typeToCreate];
    }

    public void SwipeTheGrid(GridIndex newGridIndex, Vector3 targetPosition, float swipeDuration)
    {
        gridIndex = newGridIndex;
        transform.DOMove(targetPosition, swipeDuration).SetTarget(this).SetEase(Ease.OutCubic);
    }

    public void ShakeTheGrid()
    {
        objectContainer.DOShakeRotation(0.5f, new Vector3(0, 0, 30)).SetTarget(this);
    }

    public void SetGridMatched()
    {
        transform.DOScale(1, 0.25f).SetTarget(true).SetEase(Ease.OutBack).From(0);
        objectType = GridObjectTypes.Matched;
        objectSprite.sprite = typeSprites[(int)objectType];
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
