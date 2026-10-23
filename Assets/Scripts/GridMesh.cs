using UnityEngine;
using System.Collections.Generic;

// [RequireComponent(typeof(MeshRenderer))]
// [RequireComponent(typeof(MeshFilter))]
public class GridMesh : MonoBehaviour
{
    public static GridMesh Instance; 

    public MeshFilter filter;
    //public Vector2Int gridSize;
    //public float gridSpacing = 0;

    Mesh mesh;
    List<Vector3> verticies;
    List<int> indices;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        mesh = new Mesh();
        MeshRenderer meshRenderer = filter.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = Color.black;
    }

    public Mesh CreateMesh()
    {
        mesh = new Mesh();
        MeshRenderer meshRenderer = filter.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = Color.black;
        return mesh;
    }

    public void GenerateGrid<T>(Grid2D<T> grid, float gridSpacing) 
    {
        verticies = new List<Vector3>();
        indices = new List<int>();

        Vector2Int gridSize = grid.Size;

        float xMin = gridSpacing * gridSize.x / 2;
        float zMin = gridSpacing * gridSize.y / 2;

        for (int i = 0; i <= gridSize.x; i++) {
            for (int j = 0; j <= gridSize.y; j++) {
                float x1 = i * gridSpacing - xMin;
                float x2 = (i + 1) * gridSpacing - xMin;
                float z1 = j * gridSpacing - zMin;
                float z2 = (j + 1) * gridSpacing - zMin;

                if (i != gridSize.x) {
                    verticies.Add(new Vector3(x1, 0, z1));
                    verticies.Add(new Vector3(x2, 0, z1));
                }

                if (j != gridSize.y) {
                    verticies.Add(new Vector3(x1, 0, z1));
                    verticies.Add(new Vector3(x1, 0, z2));
                }
            }
        }

        int indicesCount = verticies.Count;
        for (int i = 0; i < indicesCount; i++) {
            indices.Add(i);
        }

        mesh.vertices = verticies.ToArray();
        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
        filter.mesh = mesh;
    }
}
