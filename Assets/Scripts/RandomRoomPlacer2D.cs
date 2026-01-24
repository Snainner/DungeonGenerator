using NUnit.Framework;
using System.Collections.Generic;
using Random = System.Random;
using UnityEngine;

public class RandomRoomPlacer2D : MonoBehaviour
{
    enum CellType
    {
        Empty,
        Room,
        hallway
    }

    class Room
    {
        public RectInt bounds;
        public Room(Vector2Int location, Vector2Int size)
        {
            bounds = new RectInt(location, size);
        }

        public static bool Intersects(Room a, Room b)
        {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) ||
                     (b.bounds.position.x >= (a.bounds.position.x + a.bounds.size.x)) ||
                     (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) ||
                     (b.bounds.position.y >= (a.bounds.position.y + a.bounds.size.y)));
        }
    }

    [SerializeField] Vector2Int gridSize;
    [SerializeField] int roomCount = 10;
    [SerializeField] Vector2Int roomSizeMax;
    [SerializeField] GameObject cubePrefab;
    [SerializeField] Material redMaterial;
    [SerializeField] Material blueMaterial;

    Random random;
    Grid2D<CellType> grid;
    List<Room> rooms;

    private void Start()
    {
        Generate();
    }

    void Generate()
    {
        random = new Random(0);
        grid = new Grid2D<CellType>(gridSize, Vector2Int.zero);
        rooms = new List<Room>();

        PlaceRooms();
    }

    void PlaceRooms()
    {

        //Generate room locations and sizes
        for (int i = 0; i < roomCount; i++)
        {
            Vector2Int location = new Vector2Int(
                random.Next(0, gridSize.x),
                random.Next(0, gridSize.y)
            );

            Vector2Int roomSize = new Vector2Int(
                random.Next(1, roomSizeMax.x+1),
                random.Next(1, roomSizeMax.y+1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2)); // Add buffer to avoid rooms touching

            // Check intersections
            foreach (var room in rooms)
            {
                if (Room.Intersects(room, buffer))
                {
                    Debug.Log("Intersection detected");
                    add = false;
                    break;
                }
            }

            // Check bounds
            if (newRoom.bounds.xMin < 0 ||
                newRoom.bounds.xMax >= gridSize.x ||
                newRoom.bounds.yMin < 0 ||
                newRoom.bounds.yMax >= gridSize.y)
            {
                add = false;
                Debug.Log("Out of bounds detected");
            }

            if(add)
            {
                Debug.Log("Placing room at " + location + " size " + roomSize);
                rooms.Add(newRoom);
                PlaceRoom(location, roomSize);

                foreach(var pos in newRoom.bounds.allPositionsWithin)
                {
                    grid[pos] = CellType.Room;
                }

            }
        }

    }

    void PlaceCube(Vector2Int location, Vector2Int size, Material material)
    {
        GameObject gameObject = Instantiate(cubePrefab, new Vector3(location.x, 0, location.y), Quaternion.identity);
        gameObject.transform.localScale = new Vector3(size.x, 1, size.y);
        gameObject.GetComponent<MeshRenderer>().material = material;
    }
    void PlaceRoom (Vector2Int location, Vector2Int roomSize)
    {
        PlaceCube(location, roomSize, redMaterial);
    }
}
