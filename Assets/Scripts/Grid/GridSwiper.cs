using System.Collections;
using UnityEngine;

public class GridSwiper : MonoBehaviour
{
    #region Variables
    [SerializeField] private float swipeDuration = 0.25f;
    private int moveCount;
    private bool isSwiping = false;
    #endregion
    #region Components
    [SerializeField] private GridController gridController;
    private GameUIController gameUIController;
    #endregion
    #region Properties
    public bool IsSwiping => isSwiping;
    #endregion

    private void Start()
    {
        gameUIController = GameUIController.Instance;
    }

    public bool CanSwipeTheGrid(Grid grid, Direction swipeDirection)
    {
        if (isSwiping || moveCount >= gridController.MaxMoveCount) return false;

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
        PerformSwipe(grid, grid2);

        yield return new WaitForSeconds(swipeDuration);
        gridController.CheckIsRowMatch(grid.CurrentGridIndex.row);
        gridController.CheckIsRowMatch(grid2.CurrentGridIndex.row);
        isSwiping = false;
    }

    private void PerformSwipe(Grid grid1, Grid grid2)
    {
        var index1 = grid1.CurrentGridIndex;
        var index2 = grid2.CurrentGridIndex;

        //We could also do this scrolling with the positions of the elements themselves. But maybe the positions would change
        //during movement and we may have some bad positions. So instead, we will use an position matrix to go exact same position at each move

        grid1.SwipeTheGrid(index2, gridController.PositionMatrix[index2.row, index2.column], swipeDuration);
        grid2.SwipeTheGrid(index1, gridController.PositionMatrix[index1.row, index1.column], swipeDuration);

        gridController.GridMatrix[index1.row, index1.column] = grid2;
        gridController.GridMatrix[index2.row, index2.column] = grid1;

        moveCount++;
        gameUIController.UpdateMoveText(gridController.MaxMoveCount - moveCount);
        if(moveCount >= gridController.MaxMoveCount)
        {
            gameUIController.FinishTheGame(GameEndType.OutOfMove);
        }
    }
}
