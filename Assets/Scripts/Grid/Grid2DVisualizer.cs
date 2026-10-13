using UnityEngine;

public class Grid2DVisualizer : MonoBehaviour
{
    RandomRoomPlacer2D roomPlacer;
    public Grid2D<bool> grid;
    public float cellSize = 1f;
    Vector2Int gridSize;
    public bool EnableGridVisualizer;

    private void Start()
    {
        if (EnableGridVisualizer)
        {
            roomPlacer = gameObject.GetComponent<RandomRoomPlacer2D>();
            gridSize = roomPlacer.gridSize;
            grid = new Grid2D<bool>(gridSize, Vector2Int.zero);
        }
    }
    private void OnDrawGizmos()
    {
        if (EnableGridVisualizer)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    //Converts internal grid array coordinates to world coordinates
                    Vector2Int logicalPos = new Vector2Int(x, y); //Currently does not account for offset, but could be modified to do so if needed
                    Vector3 worldPos = new Vector3(logicalPos.x, 0, logicalPos.y);

                    Gizmos.color = grid[logicalPos] ? new Color(0, 1, 0, .3f) : new Color(1, 1, 1, .3f);
                    Gizmos.DrawCube(worldPos, Vector3.one * cellSize * 0.9f); // Slightly smaller than 1 to create a gap between cubes
                }
            }
        }
    }
}
