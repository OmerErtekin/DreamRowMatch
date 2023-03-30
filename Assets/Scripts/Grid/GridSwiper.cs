using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSwiper : MonoBehaviour
{
    #region Variables
    private bool isSwiping = false;

    #endregion
    #region Components
    public static GridSwiper Instance;
    private GridController gridController;
    #endregion
    #region Properties
    private Grid[,] GridMatrix => gridController.GridMatrix;
    private Vector3[,] PositionMatrix => gridController.PositionMatrix;
    public bool IsSwiping => isSwiping;
    #endregion
    private void Awake()
    {
        Instance = this;
        gridController = GetComponent<GridController>();
    }

    private void Start()
    {
        gridController.CreateGrid();
    }

    public bool CanSwipeTheGrid(Grid grid, Direction swipeDirection)
    {
        if (isSwiping) return false;
        return gridController.CanSwipeTheGrid(grid, swipeDirection);
    }


    public void SwipeTheGrid(Grid grid, Direction swipeDirection)
    {
        isSwiping = true;
        StartCoroutine(SwipeRoutine(grid, swipeDirection));
    }

    private IEnumerator SwipeRoutine(Grid grid, Direction swipeDirection)
    {
        Grid grid2 = gridController.GetGridToSwipe(grid.CurrentGridIndex, swipeDirection);

        PerformSwipe(grid, grid2, 0.25f);
        yield return new WaitForSeconds(0.25f);

        if (gridController.CheckIsRowMatch(grid.CurrentGridIndex.row) | gridController.CheckIsRowMatch(grid2.CurrentGridIndex.row))
        {
            gridController.CalculateAvailableMatchs();
            isSwiping = false;
            yield break;
        }
            
        PerformSwipe(grid, grid2, 0.25f);
        yield return new WaitForSeconds(0.25f);

        gridController.CalculateAvailableMatchs();
        isSwiping = false;
    }

    private void PerformSwipe(Grid grid1, Grid grid2, float swipeDuration)
    {
        var index1 = grid1.CurrentGridIndex;
        var index2 = grid2.CurrentGridIndex;

        grid1.SwipeTheGrid(index2, PositionMatrix[index2.row, index2.column], swipeDuration);
        grid2.SwipeTheGrid(index1, PositionMatrix[index1.row, index1.column], swipeDuration);

        GridMatrix[index1.row, index1.column] = grid2;
        GridMatrix[index2.row, index2.column] = grid1;
    }
}
