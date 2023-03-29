using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private float spacingBetweenGrids = 1f, gridSize = 1;
    private Grid[,] gridMatrix;
    public List<GridObjectTypes> gridFormation;
    public int gridHeight = 5, gridWidth = 5;
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

    private void Update()
    {

    }

    private void CreateGrid()
    {
        if (gridFormation.Count != gridHeight * gridWidth)
        {
            Debug.LogWarning("Wrong Formation!");
            return;
        }

        gridMatrix = new Grid[gridHeight, gridWidth];
        Vector3 startPoint = transform.position - new Vector3((gridWidth - 1) * spacingBetweenGrids / 2, (gridHeight - 1) * spacingBetweenGrids / 2, 0);
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                int index = j + i * gridWidth;
                var gridScript = Instantiate(gridPrefab, startPoint + new Vector3(j * spacingBetweenGrids, i * spacingBetweenGrids, 0), 
                    transform.rotation, transform).GetComponent<Grid>();
                gridMatrix[i, j] = gridScript;
                gridScript.InitializeGrid(i, j, gridFormation[index]);
            }
        }
    }

    public bool CanSwipeTheGrid(GridPosition gridPosition, Direction swipeDirection)
    {
        return swipeDirection switch
        {
            Direction.Up => gridPosition.i < gridHeight - 1 && gridMatrix[gridPosition.i + 1, gridPosition.j].ObjectType != GridObjectTypes.Matched,

            Direction.Down => gridPosition.i > 0 && gridMatrix[gridPosition.i - 1, gridPosition.j].ObjectType != GridObjectTypes.Matched,

            Direction.Right => gridPosition.j < gridWidth - 1 && gridMatrix[gridPosition.i, gridPosition.j + 1].ObjectType != GridObjectTypes.Matched,

            Direction.Left => gridPosition.j > 0 && gridMatrix[gridPosition.i, gridPosition.j - 1].ObjectType != GridObjectTypes.Matched,

            _ => false,
        };
    }

    public void SwipeTheGrid(Vector2 gridIndex, Direction swipeDirection)
    {

    }
}
