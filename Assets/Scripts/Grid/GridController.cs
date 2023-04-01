using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject gridPrefab, backgroundPrefab;
    [SerializeField] private Transform backgroudParent;
    [SerializeField] private float spacingBetweenGrids = 1f;
    private LevelData currentLevelData;
    private BackgroundTile[,] bgTileMatrix;
    private Grid[,] gridMatrix;
    private Vector3[,] positionMatrix;
    private bool isThereAvailableMatch = true;
    private List<int> matchedRows = new();
    private int[] elementsBetweenRows;
    #endregion

    #region Properties
    public Grid[,] GridMatrix => gridMatrix;
    public Vector3[,] PositionMatrix => positionMatrix;
    public int RowCount => currentLevelData.gridHeight;
    public int ColumnCount => currentLevelData.gridWidth;
    public int MaxMoveCount => currentLevelData.moveCount;
    #endregion

    #region Components
    private LevelReader levelReader;
    #endregion

    private void Start()
    {
        levelReader = GetComponent<LevelReader>();
        InitializeLevel(levelReader.Levels[PlayerPrefs.GetInt("SelectedLevel", 1) - 1]);
    }
    public void InitializeLevel(LevelData levelData)
    {
        currentLevelData = levelData;
        name = $"Level {currentLevelData.levelNumber} Grid";
        GameUIController.Instance.InitializeGameUI(currentLevelData);
        CreateGrid();
    }

    private void CreateGrid()
    {
        if (currentLevelData.gridFormation.Count != currentLevelData.gridHeight * ColumnCount)
        {
            Debug.LogWarning("Wrong Formation!");
            return;
        }

        gridMatrix = new Grid[RowCount, ColumnCount];
        positionMatrix = new Vector3[RowCount, ColumnCount];
        bgTileMatrix = new BackgroundTile[RowCount, ColumnCount];

        Vector3 startPoint = transform.position - new Vector3((ColumnCount - 1) * spacingBetweenGrids / 2, (RowCount - 1) * spacingBetweenGrids / 2, 0);
        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                int index = j + i * ColumnCount;
                Vector3 targetPosition = startPoint + new Vector3(j * spacingBetweenGrids, i * spacingBetweenGrids, 0);

                var gridScript = Instantiate(gridPrefab, targetPosition, transform.rotation, transform).GetComponent<Grid>();
                bgTileMatrix[i, j] = Instantiate(backgroundPrefab, targetPosition, transform.rotation, backgroudParent).GetComponent<BackgroundTile>();

                gridMatrix[i, j] = gridScript;
                positionMatrix[i, j] = targetPosition;
                gridScript.InitializeGrid(i, j, currentLevelData.gridFormation[index]);
            }
        }

        CheckAvailableMatches();
        GenerateBackgroundBorders();
    }

    private void GenerateBackgroundBorders()
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

    public void CheckAvailableMatches()
    {
        if (matchedRows.Count == 0) return;

        isThereAvailableMatch = MatchBetweenBot_FirstMatchedRow() || MatchBetweenTop_LastMatchedRow()
            || MatchBetweenMatchedRows();

        if (!isThereAvailableMatch)
            GameUIController.Instance.FinishTheGame(GameEndType.NoMoreMatch);
    }

    private bool MatchBetweenTop_LastMatchedRow()
    {
        if (matchedRows.Max() == RowCount - 1) return false;

        elementsBetweenRows = new int[(int)GridObjectTypes.Matched];

        for (int i = matchedRows.Max(); i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                if (gridMatrix[i, j].ObjectType != GridObjectTypes.Matched)
                    elementsBetweenRows[(int)GridMatrix[i, j].ObjectType]++;
            }
        }

        return elementsBetweenRows.Max() >= ColumnCount;
    }

    private bool MatchBetweenBot_FirstMatchedRow()
    {
        if (matchedRows.Min() == 0) return false;

        elementsBetweenRows = new int[(int)GridObjectTypes.Matched];

        for (int i = 0; i < matchedRows.Min(); i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                if (gridMatrix[i, j].ObjectType != GridObjectTypes.Matched)
                    elementsBetweenRows[(int)GridMatrix[i, j].ObjectType]++;
            }
        }

        return elementsBetweenRows.Max() >= ColumnCount;
    }

    private bool MatchBetweenMatchedRows()
    {
        if (matchedRows.Count <= 1) return false;

        for (int i = 0; i < matchedRows.Count - 1; i++)
        {
            elementsBetweenRows = new int[(int)GridObjectTypes.Matched];
            for (int j = matchedRows[i]; j < matchedRows[i + 1]; j++)
            {
                for (int k = 0; k < ColumnCount; k++)
                {
                    if (gridMatrix[j, k].ObjectType != GridObjectTypes.Matched)
                        elementsBetweenRows[(int)GridMatrix[j, k].ObjectType]++;
                }
            }
            if (elementsBetweenRows.Max() >= ColumnCount)
                return true;
        }
        return false;
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
        //Rules for swipe
        var gridIndex = grid.CurrentGridIndex;

        return swipeDirection switch
        {
            Direction.Up => gridIndex.row < RowCount - 1 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            Direction.Down => gridIndex.row > 0 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            Direction.Right => gridIndex.column < ColumnCount - 1 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            Direction.Left => gridIndex.column > 0 && GetGridToSwipe(gridIndex, swipeDirection).ObjectType != GridObjectTypes.Matched,

            _ => false,
        };
    }

    public void CheckIsRowMatch(int rowIndex)
    {
        if (ColumnCount <= 1) return;

        GridObjectTypes searchedType = GridMatrix[rowIndex, 0].ObjectType;
        bool isThereRowMatch = true;
        for (int i = 1; i < ColumnCount; i++)
        {
            if (GridMatrix[rowIndex, i].ObjectType != searchedType)
            {
                isThereRowMatch = false;
                break;
            }
        }

        if (isThereRowMatch)
        {
            matchedRows.Add(rowIndex);
            matchedRows.Sort();
            for (int i = 0; i < ColumnCount; i++)
            {
                GridMatrix[rowIndex, i].SetGridMatched();
            }

            CheckAvailableMatches();
        }
    }
}
