using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private float spacingBetweenGrids = 1f,gridSize = 1;
    private Grid[,] gridMatrix;
    public List<GridObjectTypes> gridFormation;
    public int gridHeight = 5, gridWidth = 5;
    #endregion

    #region Components
    #endregion
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
        Vector3 startPoint = transform.position + new Vector3((-gridWidth / 2f + gridSize / 2) * spacingBetweenGrids, 
            (gridHeight / 2f - gridSize / 2) * spacingBetweenGrids, 0f);
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                var gridScript = Instantiate(gridPrefab, startPoint + new Vector3(j * spacingBetweenGrids, -i * spacingBetweenGrids, 0), 
                    transform.rotation, transform).GetComponent<Grid>();
                gridMatrix[i, j] = gridScript;
                gridScript.name = $"{i} {j}";
                gridScript.InitializeGrid(gridFormation[j + i * gridWidth]);
            }
        }
    }
}
