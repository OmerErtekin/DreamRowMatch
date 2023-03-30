using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private float spacingBetweenGrids = 1f;
    [SerializeField] private List<GridObjectTypes> gridFormation1D;
    [SerializeField] private int gridRowCount = 5, gridColumnCount = 5;
    private Grid[,] gridMatrix;
    private Vector3[,] positionMatrix;

    #endregion
    #region Properties
    public Grid[,] GridMatrix => gridMatrix;
    public Vector3[,] PositionMatrix => positionMatrix;
    public int RowCount => gridRowCount;
    public int ColumnCount => gridColumnCount;
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

    public void PrintGrid()
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
