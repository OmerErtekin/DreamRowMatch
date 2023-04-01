using UnityEngine;

public class GridCreator : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject gridPrefab, backgroundPrefab;
    [SerializeField] private Transform backgroudParent,gridObjectsParent;
    [SerializeField] private float spacingBetweenGrids = 1f;
    private LevelData currentLevelData;
    private BackgroundTile[,] bgTileMatrix;
    private Grid[,] gridMatrix;
    private Vector3[,] positionMatrix;
    private int rowCount,columnCount;
    #endregion

    #region Properties
    public Grid[,] GridMatrix => gridMatrix;
    public Vector3[,] PositionMatrix => positionMatrix;
    #endregion

    public void CreateGrid(LevelData data)
    {
        SetVariables(data);
        if (data.gridFormation.Count != rowCount * columnCount)
        {
            Debug.LogWarning("Wrong Formation!");
            return;
        }

        Vector3 startPoint = transform.position - new Vector3((columnCount - 1) * spacingBetweenGrids / 2, (rowCount - 1) * spacingBetweenGrids / 2, 0);
        Vector3 targetPosition;
        Grid gridScript;
        int index;

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                index = j + i * columnCount;
                targetPosition = startPoint + new Vector3(j * spacingBetweenGrids, i * spacingBetweenGrids, 0);

                gridScript = Instantiate(gridPrefab, targetPosition, transform.rotation, gridObjectsParent).GetComponent<Grid>();
                bgTileMatrix[i, j] = Instantiate(backgroundPrefab, targetPosition, transform.rotation, backgroudParent).GetComponent<BackgroundTile>();

                gridMatrix[i, j] = gridScript;
                positionMatrix[i, j] = targetPosition;
                gridScript.InitializeGrid(i, j, currentLevelData.gridFormation[index]);
            }
        }

        GenerateBackgroundBorders();
    }

    private void GenerateBackgroundBorders()
    {
        for (int i = 0; i < rowCount; i++)
        {
            bgTileMatrix[i, 0].SetBorder(Direction.Left);
            bgTileMatrix[i, columnCount - 1].SetBorder(Direction.Right);
        }
        for (int i = 0; i < columnCount; i++)
        {
            bgTileMatrix[0, i].SetBorder(Direction.Down);
            bgTileMatrix[rowCount - 1, i].SetBorder(Direction.Up);
        }
    }

    private void SetVariables(LevelData data)
    {
        currentLevelData = data;
        rowCount = currentLevelData.gridHeight;
        columnCount = currentLevelData.gridWidth;

        gridMatrix = new Grid[rowCount, columnCount];
        positionMatrix = new Vector3[rowCount, columnCount];
        bgTileMatrix = new BackgroundTile[rowCount, columnCount];
    }
}
