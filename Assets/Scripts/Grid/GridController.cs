using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    #region Variables
    private bool isSwiping = false;
    #endregion

    #region Components
    public static GridController Instance;
    private GridCreator gridCreator;
    #endregion

    #region Properties
    private Grid[,] GridMatrix => gridCreator.GridMatrix;
    private Vector3[,] PositionMatrix => gridCreator.PositionMatrix;
    private int GridRowCount => gridCreator.RowCount;
    private int GridColumnCount => gridCreator.ColumnCount;
    #endregion
    private void Awake()
    {
        Instance = this;
        gridCreator = GetComponent<GridCreator>();
    }

    private void Start()
    {
        gridCreator.CreateGrid();
    }

    public bool CanSwipeTheGrid(Grid grid, Direction swipeDirection)
    {
        if (isSwiping) return false;

        var gridIndex = grid.CurrentGridIndex;

        return swipeDirection switch
        {
            Direction.Up => gridIndex.raw < GridRowCount - 1 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            Direction.Down => gridIndex.raw > 0 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            Direction.Right => gridIndex.column < GridColumnCount - 1 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            Direction.Left => gridIndex.column > 0 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            _ => false,
        };
    }

    private Grid GetGridToSwipe(GridIndex gridPosition, Direction swipeDirection)
    {
        return swipeDirection switch
        {
            Direction.Up => GridMatrix[gridPosition.raw + 1, gridPosition.column],

            Direction.Down => GridMatrix[gridPosition.raw - 1, gridPosition.column],

            Direction.Right => GridMatrix[gridPosition.raw, gridPosition.column + 1],

            Direction.Left => GridMatrix[gridPosition.raw, gridPosition.column - 1],

            _ => null,
        };
    }

    public void SwipeTheGrid(Grid grid, Direction swipeDirection)
    {
        isSwiping = true;
        StartCoroutine(SwipeRoutine(grid, swipeDirection));
    }

    private IEnumerator SwipeRoutine(Grid grid, Direction swipeDirection)
    {
        Grid grid2 = GetGridToSwipe(grid.CurrentGridIndex, swipeDirection);

        PerformSwipe(grid, grid2, 0.25f);
        yield return new WaitForSeconds(0.25f);

        if (CheckIsRowMatch(grid.CurrentGridIndex.raw) | CheckIsRowMatch(grid2.CurrentGridIndex.raw))
        {
            isSwiping = false;
            yield break;
        }

        PerformSwipe(grid, grid2, 0.25f);
        yield return new WaitForSeconds(0.25f);

        isSwiping = false;
    }

    private void PerformSwipe(Grid grid1, Grid grid2, float swipeDuration)
    {
        var index1 = grid1.CurrentGridIndex;
        var index2 = grid2.CurrentGridIndex;

        grid1.SwipeTheGrid(index2, PositionMatrix[index2.raw, index2.column], swipeDuration);
        grid2.SwipeTheGrid(index1, PositionMatrix[index1.raw, index1.column], swipeDuration);

        GridMatrix[index1.raw, index1.column] = grid2;
        GridMatrix[index2.raw, index2.column] = grid1;
    }

    private bool CheckIsRowMatch(int rowIndex)
    {
        GridObjectTypes searchedType = GridMatrix[rowIndex, 0].ObjectType;
        bool isThereRowMatch = true;
        for (int i = 0; i < GridColumnCount; i++)
        {
            if (GridMatrix[rowIndex, i].ObjectType != searchedType)
            {
                isThereRowMatch = false;
                break;
            }
        }

        if (isThereRowMatch)
        {
            for (int i = 0; i < GridColumnCount; i++)
            {
                GridMatrix[rowIndex, i].SetGridMatched();
            }
        }

        return isThereRowMatch;
    }
}
