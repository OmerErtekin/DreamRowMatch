using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

public class GridController : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private float spacingBetweenGrids = 1f;
    private Grid[,] gridMatrix;
    public Vector3[,] positionMatrix;
    public List<GridObjectTypes> gridFormation1D;
    public int gridRowCount = 5, gridColumnCount = 5;
    private bool isSwiping = false;
    #endregion

    #region Components
    public static GridController Instance;
    #endregion
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        if (gridFormation1D.Count != gridRowCount * gridColumnCount)
        {
            Debug.LogWarning("Wrong Formation!");
            return;
        }

        gridMatrix = new Grid[gridRowCount, gridColumnCount];
        positionMatrix = new Vector3[gridRowCount, gridColumnCount];

        Vector3 startPoint = transform.position - new Vector3((gridColumnCount - 1) * spacingBetweenGrids / 2, (gridRowCount - 1) * spacingBetweenGrids / 2, 0);
        for (int i = 0; i < gridRowCount; i++)
        {
            for (int j = 0; j < gridColumnCount; j++)
            {
                int index = j + i * gridColumnCount;
                Vector3 targetPosition = startPoint + new Vector3(j * spacingBetweenGrids, i * spacingBetweenGrids, 0);
                var gridScript = Instantiate(gridPrefab, targetPosition, transform.rotation, transform).GetComponent<Grid>();
                gridMatrix[i, j] = gridScript;
                positionMatrix[i, j] = targetPosition;
                gridScript.InitializeGrid(i, j, gridFormation1D[index]);
            }
        }
    }

    public bool CanSwipeTheGrid(Grid grid, Direction swipeDirection)
    {
        if (isSwiping) return false;

        var gridIndex = grid.CurrentGridIndex;

        return swipeDirection switch
        {
            Direction.Up => gridIndex.raw < gridRowCount - 1 && GetGridToSwipe(gridIndex,swipeDirection).ObjectType != GridObjectTypes.Matched,

            Direction.Down => gridIndex.raw > 0 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            Direction.Right => gridIndex.column < gridColumnCount - 1 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            Direction.Left => gridIndex.column > 0 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            _ => false,
        };
    }

    private Grid GetGridToSwipe(GridIndex gridPosition, Direction swipeDirection)
    {
        return swipeDirection switch
        {
            Direction.Up => gridMatrix[gridPosition.raw + 1, gridPosition.column],

            Direction.Down => gridMatrix[gridPosition.raw - 1, gridPosition.column],

            Direction.Right => gridMatrix[gridPosition.raw, gridPosition.column + 1],

            Direction.Left => gridMatrix[gridPosition.raw, gridPosition.column - 1],

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
        Grid grid2 = GetGridToSwipe(grid.gridIndex, swipeDirection);

        var index1 = grid.CurrentGridIndex;
        var index2 = grid2.gridIndex;

        grid.SwipeTheGrid(index2, positionMatrix[index2.raw, index2.column]);
        grid2.SwipeTheGrid(index1, positionMatrix[index1.raw, index1.column]);

        gridMatrix[index1.raw, index1.column] = grid2;
        gridMatrix[index2.raw, index2.column] = grid;

        yield return new WaitForSeconds(0.5f);

        PrintGrid();
        isSwiping = false;
    }

    private void PrintGrid()
    {
        string allGrid = "";
        for (int i = 0; i < gridRowCount; i++)
        {
            string line = "";
            for (int j = 0; j < gridColumnCount; j++)
            {
                line += $" {gridMatrix[i, j].name} ";
            }

            allGrid += $"{line} \t";
        }
        Debug.Log(allGrid);
    }
}
