using UnityEngine;

public class Grid2D<T>
{
    // Internal 1D array to store data 
    // The 2D grid is represented as a 1D array for efficiency and cache friendliness. The index is calculated based on the 2D coordinates.
    T[] _data;

    public Vector2Int _Size { get; private set; }

    // Offset allows for shifting the grid's origin, enabling negative coordinates or custom positioning such as centering the grid around a specific point, such as the middle fo the screen for example
    public Vector2Int _Offset { get; set; }
    public Grid2D(Vector2Int size, Vector2Int offset)
    {
        _Size = size;
        _Offset = offset;
        _data = new T[size.x * size.y];
    }

    // Convert 2D position to 1D index without offset
    int GetIndex(Vector2Int pos)
    {
        return pos.x + (_Size.x * pos.y);
    }

    // Check if position is within bounds
    public bool InBounds(Vector2Int pos)
    {
        return new RectInt(Vector2Int.zero, _Size).Contains(pos + _Offset);
    }

    // Convenience indexer
    public T this[int x, int y]
    {
        get
        {
            return this[new Vector2Int(x, y)];
        }
        set
        {
            this[new Vector2Int(x, y)] = value;
        }
    }
    // Indexer with offset
    public T this[Vector2Int pos]
    {
        get
        {
            pos += _Offset;
            return _data[GetIndex(pos)];
        }
        set
        {
            pos += _Offset;
            _data[GetIndex(pos)] = value;
        }
    }

}
