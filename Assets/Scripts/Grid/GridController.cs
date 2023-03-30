using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject gridPrefab, backgroundPrefab;
    [SerializeField] private Transform backgroudParent;
    [SerializeField] private float spacingBetweenGrids = 1f;
    [SerializeField] private List<GridObjectTypes> gridFormation1D;
    [SerializeField] private int gridRowCount = 5, gridColumnCount = 5;
    private BackgroundTile[,] bgTileMatrix;
    private Grid[,] gridMatrix;
    private Vector3[,] positionMatrix;
    private List<RowWithMaxElement> possibleRowsWithMaxElement;
    public int availableMatchCount;

    #endregion
    #region Properties
    public Grid[,] GridMatrix => gridMatrix;
    public Vector3[,] PositionMatrix => positionMatrix;
    public int RowCount => gridRowCount;
    public int ColumnCount => gridColumnCount;
    public int AvailableMatchCount => availableMatchCount;
    #endregion

    public void CreateGrid()
    {
        if (gridFormation1D.Count != gridRowCount * gridColumnCount)
        {
            Debug.LogWarning("Wrong Formation!");
            return;
        }

        gridMatrix = new Grid[gridRowCount, gridColumnCount];
        positionMatrix = new Vector3[gridRowCount, gridColumnCount];
        bgTileMatrix = new BackgroundTile[gridRowCount, gridColumnCount];

        Vector3 startPoint = transform.position - new Vector3((gridColumnCount - 1) * spacingBetweenGrids / 2, (gridRowCount - 1) * spacingBetweenGrids / 2, 0);
        for (int i = 0; i < gridRowCount; i++)
        {
            for (int j = 0; j < gridColumnCount; j++)
            {
                int index = j + i * gridColumnCount;
                Vector3 targetPosition = startPoint + new Vector3(j * spacingBetweenGrids, i * spacingBetweenGrids, 0);

                var gridScript = Instantiate(gridPrefab, targetPosition, transform.rotation, transform).GetComponent<Grid>();
                bgTileMatrix[i, j] = Instantiate(backgroundPrefab, targetPosition, transform.rotation, backgroudParent).GetComponent<BackgroundTile>();

                gridMatrix[i, j] = gridScript;
                positionMatrix[i, j] = targetPosition;
                gridScript.InitializeGrid(i, j, gridFormation1D[index]);
            }
        }

        CalculateAvailableMatchs();
        GenerateBorders();
    }

    private void GenerateBorders()
    {
        for (int i = 0; i < RowCount; i++)
        {
            bgTileMatrix[i, 0].SetBorder(Direction.Left);
            bgTileMatrix[i, ColumnCount - 1].SetBorder(Direction.Right);
        }
        for (int i = 0; i < ColumnCount; i++)
        {
            bgTileMatrix[0, i].SetBorder(Direction.Down);
            bgTileMatrix[RowCount - 1, i].SetBorder(Direction.Up);
        }
    }

    public void CalculateAvailableMatchs()
    {
        if (gridColumnCount <= 1) return;
        availableMatchCount = 0;

        SetPossibleRowAndElements();
        if (possibleRowsWithMaxElement.Count == 0)
            return;


        for (int i = 0; i < possibleRowsWithMaxElement.Count; i++)
        {
            if (CanMatchWithDirection(i, Direction.Up) || CanMatchWithDirection(i, Direction.Down))
            {
                availableMatchCount++;
            }
        }

    }

    private bool CanMatchWithDirection(int possibleRowIndex, Direction direction)
    {
        var grid = GetDifferentElementOnRow(possibleRowsWithMaxElement[possibleRowIndex]);

        return CanSwipeTheGrid(grid, direction) &&
            GetGridToSwipe(grid.CurrentGridIndex, direction).ObjectType == possibleRowsWithMaxElement[possibleRowIndex].MaxElementType;
    }

    private void SetPossibleRowAndElements()
    {
        possibleRowsWithMaxElement = new List<RowWithMaxElement>();

        for (int i = 0; i < gridRowCount; i++)
        {
            int[] rowElements = new int[5];

            for (int j = 0; j < gridColumnCount; j++)
            {
                if (GridMatrix[i, j].ObjectType != GridObjectTypes.Matched)
                    rowElements[(int)GridMatrix[i, j].ObjectType]++;
            }

            for (int k = 0; k < rowElements.Length; k++)
            {
                if (rowElements[k] == gridColumnCount - 1)
                {
                    RowWithMaxElement possibleElement = new()
                    {
                        RowNumber = i,
                        MaxElementType = (GridObjectTypes)Enum.ToObject(typeof(GridObjectTypes), k)
                    };
                    possibleRowsWithMaxElement.Add(possibleElement);
                    break;
                }
            }
        }
    }

    private Grid GetDifferentElementOnRow(RowWithMaxElement rowWithElement)
    {
        for (int i = 0; i < gridColumnCount; i++)
        {
            if (gridMatrix[rowWithElement.RowNumber, i].ObjectType != rowWithElement.MaxElementType)
                return gridMatrix[rowWithElement.RowNumber, i];
        }
        return null;
    }

    public Grid GetGridToSwipe(GridIndex gridPosition, Direction swipeDirection)
    {
        return swipeDirection switch
        {
            Direction.Up => GridMatrix[gridPosition.row + 1, gridPosition.column],

            Direction.Down => GridMatrix[gridPosition.row - 1, gridPosition.column],

            Direction.Right => GridMatrix[gridPosition.row, gridPosition.column + 1],

            Direction.Left => GridMatrix[gridPosition.row, gridPosition.column - 1],

            _ => null,
        };
    }

    public bool CanSwipeTheGrid(Grid grid, Direction swipeDirection)
    {

        var gridIndex = grid.CurrentGridIndex;

        return swipeDirection switch
        {
            Direction.Up => gridIndex.row < gridRowCount - 1 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            Direction.Down => gridIndex.row > 0 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            Direction.Right => gridIndex.column < gridColumnCount - 1 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            Direction.Left => gridIndex.column > 0 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            _ => false,
        };
    }
    public bool CheckIsRowMatch(int rowIndex)
    {
        if (gridColumnCount <= 1) return false;

        GridObjectTypes searchedType = GridMatrix[rowIndex, 0].ObjectType;
        bool isThereRowMatch = true;
        for (int i = 1; i < gridColumnCount; i++)
        {
            if (GridMatrix[rowIndex, i].ObjectType != searchedType)
            {
                isThereRowMatch = false;
                break;
            }
        }

        if (isThereRowMatch)
        {
            for (int i = 0; i < gridColumnCount; i++)
            {
                GridMatrix[rowIndex, i].SetGridMatched();
            }
        }

        return isThereRowMatch;
    }
}
[Serializable]
public struct RowWithMaxElement
{
    public int RowNumber;
    public GridObjectTypes MaxElementType;
}
