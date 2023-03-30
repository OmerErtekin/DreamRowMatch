using System;


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

