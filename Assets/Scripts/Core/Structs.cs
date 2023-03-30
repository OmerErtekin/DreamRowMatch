using System;
using System.Collections.Generic;

[Serializable]
public struct RowWithMaxElement
{
    public int RowNumber;
    public GridObjectTypes MaxElementType;
}

[Serializable]
public struct GridIndex
{
    public int row; public int column;
}

[Serializable] 
public struct LevelData
{
    public int levelNumber;
    public int gridWidth, gridHeight;
    public int moveCount;
    public List<GridObjectTypes> gridFormation;
}
