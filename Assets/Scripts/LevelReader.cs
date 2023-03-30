using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LevelReader : MonoBehaviour
{
    public string directoryPath;
    public List<LevelData> levels;
    public GridController gridController;
    public int index = 0;
    void Start()
    {
        ReadAllDownloadedLevels();
        gridController.InitializeLevel(levels[index]);
    }

    private void ReadAllDownloadedLevels()
    {
        DirectoryInfo dir = new DirectoryInfo(directoryPath);
        FileInfo[] files = dir.GetFiles("*.*");

        foreach (FileInfo file in files)
        {
            if (!string.IsNullOrEmpty(file.Extension)) continue;

            string[] lines = File.ReadAllLines(file.FullName);

            int levelNumber = int.Parse(lines[0].Split(':')[1].Trim());
            int gridWidth = int.Parse(lines[1].Split(':')[1].Trim());
            int gridHeight = int.Parse(lines[2].Split(':')[1].Trim());
            int moveCount = int.Parse(lines[3].Split(':')[1].Trim());
            string[] gridColors = lines[4].Split(':')[1].Trim().Split(',');

            List<GridObjectTypes> grid = new List<GridObjectTypes>();
            for (int i = 0; i < gridColors.Length; i++)
            {
                string color = gridColors[i].Trim();
                switch (color)
                {
                    case "r":
                        grid.Add(GridObjectTypes.Red);
                        break;
                    case "g":
                        grid.Add(GridObjectTypes.Green);
                        break;
                    case "y":
                        grid.Add(GridObjectTypes.Yellow);
                        break;
                    case "b":
                        grid.Add(GridObjectTypes.Blue);
                        break;
                    default:
                        Debug.LogWarning("Unknown color: " + color);
                        break;
                }
            }

            LevelData newLevel = new()
            {
                levelNumber = levelNumber,
                gridWidth = gridWidth,
                gridHeight = gridHeight,
                moveCount = moveCount,
                gridFormation = grid
            };

            levels.Add(newLevel);
        }

        levels = levels.OrderBy(x => x.levelNumber).ToList();
    }

}