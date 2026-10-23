using NUnit.Framework;
using System.Collections.Generic;
using Random = System.Random;
using UnityEngine;
using System.Collections;
using System.Linq;

public class Generator2D : MonoBehaviour
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

            return 
            !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || 
            ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x) || 
            (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || 
            ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y));

          
        }
    }

    [SerializeField] Vector2Int _gridSize;
    [SerializeField] int _roomCount = 100;
    [SerializeField] Vector2Int _roomSizeMax;
    [SerializeField] bool _spawnBufferMesh; 
    [SerializeField] int _edgesToAddBack;

    [SerializeField] GameObject _cubePrefab;
    [SerializeField] Material _redMaterial;
    [SerializeField] Material _blueMaterial;


    Random _random;
    int generatedRoomCount;
    Grid2D<CellType> _grid;
    List<Room> _rooms = new List<Room>();
    List<Room> _bufferRooms = new List<Room>();
    List<GameObject> _roomMeshes = new List<GameObject>();
    List<GameObject> _bufferMeshes = new List<GameObject>();
    List<Triangle> _triangulation = new List<Triangle>();
    List<Edge> _MSP = new List<Edge>();
    bool bufferActive = true;

    private void Start()
    {
        Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        _random = new Random();
        _grid = new Grid2D<CellType>(_gridSize, Vector2Int.zero);
        _rooms = new List<Room>();
        
        //GridMesh.Instance.GenerateGrid(_grid, 1);

        ClearAll();
        GenerateRooms();
    }
    [ContextMenu("Clear")]
    void ClearAll()
    {
        for (int i = 0; i < _roomMeshes.Count; i++)
        {
            Debug.Log($"Removing {_roomMeshes[i]}");
            Destroy(_roomMeshes[i]);
        }
        _rooms.Clear();
        _roomMeshes.Clear();
        generatedRoomCount = 0;
    }

    void GenerateRooms()
    {
        //Generate room locations and sizes
        for (int i = 0; i < _roomCount; i++)
        {
            Vector2Int location = new Vector2Int(
                _random.Next(0, _gridSize.x),
                _random.Next(0, _gridSize.y)
            );

            Vector2Int roomSize = new Vector2Int(
                _random.Next(1, _roomSizeMax.x+1),
                _random.Next(1, _roomSizeMax.y+1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room newBuffer = new Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2)); // Add buffer to avoid rooms touching

            // Check intersections
            foreach (var buffer in _bufferRooms)
            {
                if (Room.Intersects(buffer, newBuffer))
                {
                    //Debug.Log("Intersection detected");
                    add = false;
                    break;
                }
            }

            // if(ColliderOverlapCheck(newRoom))
            // {
            //     add = false; 
            //     break;
            // }

            // Check bounds
            if (newRoom.bounds.xMin < 0 ||
                newRoom.bounds.xMax >= _gridSize.x ||
                newRoom.bounds.yMin < 0 ||
                newRoom.bounds.yMax >= _gridSize.y)
            {
                add = false;
                //Debug.Log("Out of bounds detected");
            }

            if(add)
            {
                //Debug.Log("Placing room at " + location + " size " + roomSize);
                generatedRoomCount++;
                _rooms.Add(newRoom);
                _bufferRooms.Add(newBuffer);

                PlaceCube(location, roomSize, _redMaterial, generatedRoomCount);

                if(_spawnBufferMesh)
                _bufferMeshes.Add(PlaceCube(location, newBuffer.bounds.size, _blueMaterial, generatedRoomCount));

                foreach(var pos in newRoom.bounds.allPositionsWithin)
                {
                    _grid[pos] = CellType.Room;
                }

            }
        }
        StartCoroutine(Triangulate());
    }

    GameObject PlaceCube(Vector2Int location, Vector2Int size, Material material, int id)
    {
        //RectInt start at the bottom left corner but when Unity scales an object it does it from the center
        //Therefor the location needs to be centred with the size before instantiating 
        Vector3 centrePosition = new Vector3(location.x + size.x /2, 0, location.y + size.y /2);


        GameObject gameObject = Instantiate(_cubePrefab ,centrePosition, Quaternion.identity);
        gameObject.name = "prefab_" + id;

        gameObject.transform.localScale = new Vector3(size.x, 1, size.y);
        gameObject.GetComponent<MeshRenderer>().material = material;
        _roomMeshes.Add(gameObject);
        return gameObject;
    }
    void PlaceRoom (Vector2Int location, Vector2Int roomSize)
    {
        //PlaceCube(location, roomSize, _redMaterial);
    }

    [ContextMenu("DisableBuffer")]
    public void DisableBuffer()
    {
       
        foreach(var buffer in _bufferMeshes)
        {
            if (bufferActive)
            {
                buffer.SetActive(false);
                bufferActive = false;
            }
            else
            {
                buffer.SetActive(true);
                bufferActive = true;
            }
        }
        
       
    }

    bool ColliderOverlapCheck(Room room)
    {
        //Set to one as even one collision is enough to cancel 
        Collider[] collidersHit = new Collider[1];
        Vector3 halfExtend = new Vector3(room.bounds.size.x, 1, room.bounds.size.y);
        int numberOfCollidersHit = Physics.OverlapBoxNonAlloc(new Vector3(room.bounds.position.x, 0, room.bounds.y), halfExtend, collidersHit, Quaternion.identity);
        Debug.Log($"Number of hits: {numberOfCollidersHit}");

        if (numberOfCollidersHit == 0)
        {

            return false;
        }
        else
        {
            
            return true; 
        }
    }

    IEnumerator Triangulate()
    {
        List<Vector2> points = new List<Vector2>();
        foreach (Room room in _rooms)
        {
            points.Add(new Vector2(room.bounds.center.x, room.bounds.center.y));
        }
        //Debug.Log($"Point count:{points.Count}");
        _triangulation = Traingulation2D.Triangulate(points); 
        //Debug.Log($"Triangle Count: {_triangulation.Count}");
        
        yield return StartCoroutine(CreateMSP());
    }

    IEnumerator CreateMSP()
    {
        var edges = Traingulation2D.ConvertTrianglesToEdges(_triangulation);
        
        _MSP = MSP.Prim(edges,edges[1].A);
        MSP.AddRandomEdgesBack(edges, _MSP, _edgesToAddBack);
        yield return null;
    }

    IEnumerator StartPathFinding()
    {
        PathFinding2D pathFinder = new PathFinding2D(_gridSize);

        foreach (var edge in _MSP)
        {
            //var startPos = startRoom
        }
        yield return null;
    } 

    void ConvertTriangleIntoEdges(Triangle triangle)
    {
        new Vector2(triangle.A.x, triangle.A.y);
    }

    private void OnDrawGizmos()
    {
        if (_triangulation.Count <= 0)
        {
            //Debug.LogWarning("No triangles to draw");
            //return;
        }
        foreach (Triangle triangle in _triangulation)
        {
            Vector3 a = new Vector3(triangle.A.x, 1, triangle.A.y);
            Vector3 b = new Vector3(triangle.B.x, 1, triangle.B.y);
            Vector3 c = new Vector3(triangle.C.x, 1,triangle.C.y);
            
            Gizmos.color = Color.green;

            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, a);
        }

        foreach(Edge edge in _MSP)
        {
            Gizmos.color = Color.honeydew;
            Gizmos.DrawLine(new Vector3(edge.A.x, 1, edge.A.y), new Vector3(edge.B.x, 1, edge.B.y));
        }

        foreach (Room room in _rooms)
        {
            Gizmos.color = Color.aquamarine;
            Gizmos.DrawSphere(new Vector3(room.bounds.center.x, .5f, room.bounds.center.y), .2f);
        }
    }

}
