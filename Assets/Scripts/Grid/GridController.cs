using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : MonoBehaviour
{
    #region Variables
    private LevelData currentLevelData;
    private bool isThereAvailableMatch = true;
    private List<int> matchedRows = new();
    private int[] elementsBetweenRows;
    #endregion

    #region Properties
    public Grid[,] GridMatrix => gridCreator.GridMatrix;
    public Vector3[,] PositionMatrix => gridCreator.PositionMatrix;
    public int RowCount => currentLevelData.gridHeight;
    public int ColumnCount => currentLevelData.gridWidth;
    public int MaxMoveCount => currentLevelData.moveCount;
    #endregion

    #region Components
    private GridCreator gridCreator;
    private LevelReader levelReader;
    #endregion

    private void Start()
    {
        gridCreator = GetComponent<GridCreator>();
        levelReader = GetComponent<LevelReader>();
        InitializeLevel(levelReader.Levels[PlayerPrefs.GetInt("SelectedLevel", 1) - 1]);
    }

    public void InitializeLevel(LevelData levelData)
    {
        currentLevelData = levelData;
        name = $"Level {currentLevelData.levelNumber} Grid";
        GameUIController.Instance.InitializeGameUI(currentLevelData);
        gridCreator.CreateGrid(levelData);
    }

    public void CheckAvailableMatches()
    {
        if (matchedRows.Count == 0) return;
        //I made a simple match control without checking is there enough move to make this match happen.
        //I think i could also do that by creating decision trees with N = remained Move Count but i don't think that that's what you wanted so i made it simple.
        isThereAvailableMatch = MatchBetweenBot_BotMatchedRow() || MatchBetweenTop_TopMatchedRow()
            || MatchBetweenMatchedRows();

        if (!isThereAvailableMatch)
            GameUIController.Instance.FinishTheGame(GameEndType.NoMoreMatch);
    }

    private bool MatchBetweenTop_TopMatchedRow()
    {
        //Check is there enough elements to make match between top edge and 
        //the top matched row
        if (matchedRows.Max() == RowCount - 1) return false;

        elementsBetweenRows = new int[(int)GridObjectTypes.Matched];

        for (int i = matchedRows.Max(); i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                if (gridCreator.GridMatrix[i, j].ObjectType != GridObjectTypes.Matched)
                    elementsBetweenRows[(int)gridCreator.GridMatrix[i, j].ObjectType]++;
            }
        }
        return elementsBetweenRows.Max() >= ColumnCount;
    }

    private bool MatchBetweenBot_BotMatchedRow()
    {
        //Check is there enough elements to make match between bot edge and
        //the bot matched row
        if (matchedRows.Min() == 0) return false;

        elementsBetweenRows = new int[(int)GridObjectTypes.Matched];

        for (int i = 0; i < matchedRows.Min(); i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                if (gridCreator.GridMatrix[i, j].ObjectType != GridObjectTypes.Matched)
                    elementsBetweenRows[(int)gridCreator.GridMatrix[i, j].ObjectType]++;
            }
        }
        return elementsBetweenRows.Max() >= ColumnCount;
    }

    private bool MatchBetweenMatchedRows()
    {
        //Check is there enough elements to make match between matched rows
        if (matchedRows.Count <= 1) return false;

        for (int i = 0; i < matchedRows.Count - 1; i++)
        {
            elementsBetweenRows = new int[(int)GridObjectTypes.Matched];
            for (int j = matchedRows[i]; j < matchedRows[i + 1]; j++)
            {
                for (int k = 0; k < ColumnCount; k++)
                {
                    if (gridCreator.GridMatrix[j, k].ObjectType != GridObjectTypes.Matched)
                        elementsBetweenRows[(int)gridCreator.GridMatrix[j, k].ObjectType]++;
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
            Direction.Up => gridCreator.GridMatrix[gridPosition.row + 1, gridPosition.column],

            Direction.Down => gridCreator.GridMatrix[gridPosition.row - 1, gridPosition.column],

            Direction.Right => gridCreator.GridMatrix[gridPosition.row, gridPosition.column + 1],

            Direction.Left => gridCreator.GridMatrix[gridPosition.row, gridPosition.column - 1],

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

    public void TryMatchTheRow(int rowIndex)
    {
        if (ColumnCount <= 1) return;

        GridObjectTypes searchedType = gridCreator.GridMatrix[rowIndex, 0].ObjectType;
        for (int i = 1; i < ColumnCount; i++)
        {
            if (gridCreator.GridMatrix[rowIndex, i].ObjectType != searchedType)
            {
                return;
            }
        }
        matchedRows.Add(rowIndex);
        matchedRows.Sort();
        for (int i = 0; i < ColumnCount; i++)
        {
            gridCreator.GridMatrix[rowIndex, i].SetGridMatched();
        }
        CheckAvailableMatches();
    }
}
