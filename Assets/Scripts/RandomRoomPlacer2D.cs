using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Random = System.Random;
public struct GenerationData2D
{
    public List<Room> rooms;
    public List<GameObject> gameObjects;
    public List<Vector3> locations;
    //public List<Edge> edges;

    public GenerationData2D(List<Room> rooms, List<GameObject> gameObjects,  List<Vector3> locations)
    {
        this.rooms = rooms;
        this.gameObjects = gameObjects;
        this.locations = locations;
    }
}
public class RoomData
{
    public RectInt bounds;
    public RoomData(Vector2Int location, Vector2Int size)
    {
        bounds = new RectInt(location, size);
    }

    public static bool Intersects(RoomData a, RoomData b)
    {
        return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) ||
                 (b.bounds.position.x >= (a.bounds.position.x + a.bounds.size.x)) ||
                 (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) ||
                 (b.bounds.position.y >= (a.bounds.position.y + a.bounds.size.y)));
    }
    
}
public class RandomRoomPlacer2D : MonoBehaviour
{
    enum CellType
    {
        Empty,
        Room,
        hallway
    }

    public Vector2Int gridSize;
    [SerializeField] int roomCount = 10;
    [SerializeField] Vector2Int roomSizeMax;
    [SerializeField] Vector2Int roomSizeBuffer = new Vector2Int(2,2);
    [SerializeField] Vector2Int locationBuffer = new Vector2Int (1,1);
    [SerializeField] GameObject cubePrefab;
    [SerializeField] Material redMaterial;
    [SerializeField] Material blueMaterial;

    Random random;
    Grid2D<CellType> grid;
    List<RoomData> roomsData;
    public List<GameObject> roomObjects = new List<GameObject>();
    public List<Room> rooms = new List<Room>();
    public List<Vector3> roomLocations = new List<Vector3>();   
    public GenerationData2D generationData { private set;  get; }
    Triangle superTriangle;
    bool drawSuperTriangle = false;

    Transform roomsParent;
    public void Regen()
    {
        //Clear existing cubes 
        ClearDungeon();
        Generate();
        CheckOverlapping();
        //clear lists 
        //trigger generate
    }

    public void Generate()
    {
        if(roomsParent == null)
        {
            roomsParent = Instantiate(new GameObject(name = "Rooms")).transform;
        }

        random = new Random();
        grid = new Grid2D<CellType>(gridSize, Vector2Int.zero); 
        roomsData = new List<RoomData>();

        PlaceRooms();
        CheckOverlapping();
        CreateSuperTriangle();
        //if(roomsData.Count > 0) 
        //    Regen();
    }

    void CreateSuperTriangle()
    {
        superTriangle = BowyerWatson.SuperTriangle(roomLocations);
        drawSuperTriangle = true;
        Debug.Log($"SuperTriangle Points: <br>A: {superTriangle.PointA.Location} <br>B: {superTriangle.PointB.Location} <br>C: {superTriangle.PointA.Location}");
    }
    void ClearDungeon()
    {
        if (roomsData != null && roomsData.Count > 0)
        {
            roomsData.Clear();
            //roomObjects.ForEach(obj => Destroy(obj.gameObject));
            roomObjects.Clear();
            roomLocations.Clear();

            rooms.ForEach(e => e.DestroyRoom());
            rooms.Clear();
            generationData = new GenerationData2D();
            drawSuperTriangle = false;
        }
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
            RoomData newRoom = new RoomData(location, roomSize);
            RoomData buffer = new RoomData(location - locationBuffer, roomSize + roomSizeBuffer); // Add buffer to avoid rooms touching

            // Check intersections
            foreach (var room in roomsData)
            {
                if (RoomData.Intersects(room, buffer))
                {
                    
                    //Show ghost object for a second to visualize the attempt
                    Debug.Log("<color=red><b>Generator:</b></color> Intersection detected");
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
                Debug.Log("<color=red><b>Generator:</b></color> Out of bounds detected");
            }

            if(add)
            {
                //Debug.Log("Placing room at " + location + " size " + roomSize);
                roomsData.Add(newRoom);
                PlaceRoom(location, roomSize);

                foreach(var pos in newRoom.bounds.allPositionsWithin)
                {
                    grid[pos] = CellType.Room;
                }

            }
        }
    }

    public void CheckOverlapping()
    {
        int maxIterations = 100;
        for(int i = 0; i < maxIterations; i++)
        {
            if (!RoomValidator.PushRoomsApart(rooms, 2f))
            {
                roomLocations = rooms.Select(e => e.Bounds.center).ToList();
                generationData = new GenerationData2D { gameObjects = roomObjects, locations = roomLocations, rooms = rooms };
                Debug.Log($"<color=green><b>Generator:</b></color> No overlaps detected in {i} passes <br>{roomLocations.Count} Room locations added");
                break;
            }
        }
    }

    GameObject PlaceCube(Vector2Int location, Vector2Int size, Material material)
    {
        //Centre the cube's pivot point at the center of the room by offsetting the position by half the size of the cube
        //Vector3 centre = new Vector3(location.x + size.x / 2f, 0, location.y + size.y / 2f);

        GameObject gameObject = Instantiate(cubePrefab, new Vector3(location.x, 0, location.y), Quaternion.identity, roomsParent);
        gameObject.transform.localScale = new Vector3(size.x, 1, size.y);
        gameObject.GetComponent<MeshRenderer>().material = material;

        return gameObject;
    }
    void PlaceRoom (Vector2Int location, Vector2Int roomSize)
    {
        GameObject roomGo = PlaceCube(location, roomSize, redMaterial);
        Room roomComponent = roomGo.AddComponent<Room>();
        int roomIndex = roomObjects.Count + 1;
        roomComponent._Index = roomIndex;
        roomComponent.Init(new Vector3(roomSize.x, 1, roomSize.y));
        roomGo.name = $"room_{roomIndex}";
        roomGo.layer = LayerMask.NameToLayer("Room");
        
        rooms.Add(roomComponent);
        roomObjects.Add(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellowGreen;
        if(drawSuperTriangle)
        {
            Handles.Label(superTriangle.PointA.Location, "A");
            Handles.Label(superTriangle.PointB.Location, "B");
            Handles.Label(superTriangle.PointC.Location, "C");

            Gizmos.DrawLine(superTriangle.PointA.Location, superTriangle.PointB.Location);
            Gizmos.DrawLine(superTriangle.PointB.Location, superTriangle.PointC.Location);
            Gizmos.DrawLine(superTriangle.PointC.Location, superTriangle.PointA.Location);

            Gizmos.color = Color.yellow;
            foreach (var loc in roomLocations)
                Gizmos.DrawSphere(loc, .5f);
            Gizmos.color = Color.green;
            foreach (var loc in generationData.locations)
                Gizmos.DrawSphere(loc, .5f);
        }
    }
}
