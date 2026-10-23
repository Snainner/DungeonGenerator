using UnityEngine;
using System.Collections.Generic;

public class Traingulation2D : MonoBehaviour
{

    public static List <Triangle> Triangulate(List<Vector2> points)
    {
        Triangle _superTriangle = CreateSuperTriangle(points);
        List<Triangle> _triangulation = new List<Triangle> {_superTriangle};
        

        foreach(Vector2 point in points)
        {
            List<Triangle> badTriangles = new List<Triangle>();
            
            //Find all triangles that contain this point in their cirumcircle 
            foreach(Triangle triangle in _triangulation)
            {
                if(triangle.IsPointInCircumcircle(point))
                {
                    badTriangles.Add(triangle);
                }
            }
           
            //Find the boundery edges of the polygonal hole
            List<Edge> polygon = new List<Edge>();

            foreach (Triangle triangle in badTriangles)
            {
                Edge[] edges = new Edge[]
                {
                    new Edge(triangle.A, triangle.B),
                    new Edge(triangle.B, triangle.C),
                    new Edge(triangle.C, triangle.A)
                };

                foreach (Edge edge in edges)
                {
                    bool isShared = false; 

                    foreach(Triangle otherTriangle in badTriangles)
                    {
                        if(triangle == otherTriangle)
                            continue;
                        
                        if((otherTriangle.ContainsVertex(edge.A) && otherTriangle.ContainsVertex(edge.B)))
                        {
                            isShared = true;
                            break;
                        }

                    }
                        if(!isShared)
                        {
                            bool alreadyExists = false;
                            foreach (Edge existingEdge in polygon)
                            {
                                if(existingEdge.Equals(edge))
                                {
                                    alreadyExists = true;
                                    break;
                                }
                            }

                            if(!alreadyExists)
                            {
                                polygon.Add(edge);
                            }
                        }
                }
            }

            //Remove the bad triangles 
            //Debug.Log($"Bad triangle count: {badTriangles.Count}");
            foreach(Triangle triangle in badTriangles)
            {
                _triangulation.Remove(triangle); 
            }
            //Create new triangles from the point to each edge 
            foreach(Edge edge in polygon)
            {
                _triangulation.Add(new Triangle(edge.A, edge.B, point));
            }
        }

        //Remove triangles connected to the super triangle
        _triangulation.RemoveAll(triangle => triangle.ContainsVertex(_superTriangle.A)|| triangle.ContainsVertex(_superTriangle.B)|| triangle.ContainsVertex(_superTriangle.C));
        
        

        return _triangulation;
    }

    static Triangle CreateSuperTriangle(List<Vector2> points)
    {
        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        foreach(Vector2 point in points)
        {
            minX = Mathf.Min(minX, point.x);
            minY = Mathf.Min(minY, point.y);
            maxX = Mathf.Max(maxX, point.x);
            maxY = Mathf.Max(maxY, point.y);
        }

        float dx = maxX - minX;
        float dy = maxY - minY;
        float deltaMax = Mathf.Max(dx, dy) * 2;

        Vector2 p1 = new Vector2(minX - 1, minY-1);
        Vector2 p2 = new Vector2(minX -1, maxY + deltaMax);
        Vector2 p3 = new Vector2(maxX + deltaMax, minY-1);
     
        return new Triangle(p1, p2, p3);
    }

    public static List<Edge> ConvertTrianglesToEdges(List<Triangle> triangles)
    {
        //Check if an edge has already been added by checking the hash set
        //if it isn't in there yet we can add it to the actual list for edges
        HashSet<Edge> edgeSet = new HashSet<Edge>();
        List<Edge> edges = new List<Edge>();
        foreach (Triangle triangle in triangles)
        {
            Edge ab = new Edge(triangle.A, triangle.B);
            Edge bc = new Edge(triangle.B, triangle.C);
            Edge ca = new Edge(triangle.C, triangle.A);

            if (edgeSet.Add(ab))
            {
                edges.Add(ab);
            }
            if(edgeSet.Add(bc))
            {
                edges.Add(bc);
            }
            if (edgeSet.Add(ca))
            {
                edges.Add(ca);
            }
        }
        return edges;
    }
}


public class Edge
{
    public Vector2 A, B; 
    public Edge(Vector2 a, Vector2 b)
    {
        this.A = a;
        this.B = b;
    }

    public float Distance()
    {
        return Vector2.Distance(A, B);
    }

    public bool Equals(Edge other)
    {
        return (A == other.A && B == other.B) || (A == other.B && B == other.A);  
    }
}

public class Triangle
{
    public Vector2 A, B, C;
    public Circle Circumcircle;

    public Triangle(Vector2 a, Vector2 b, Vector2 c)
    {
        this.A = a;
        this.B = b;
        this.C = c;
        this.Circumcircle = CalculateCircumcircle();
    }

    Circle CalculateCircumcircle()
    {
        float d = 2 * (A.x *(B.y- C.y)+ B.x * (C.y- A.y) + C.x *(A.y - B.y));

        float ux = ((A.x * A.x + A.y* A.y) * (B.y-C.y) +
                    (B.x * B.x + B.y * B.y) *(C.y - A.y)+
                    (C.x * C.x + C.y *C.y)* (A.y - B.y)) / d;
        
        float uy = ((A.x * A.x + A.y * A.y) * (C.x - B.x)+
                    (B.x * B.x + B.y * B.y) * (A.x - C.x)+ 
                    (C.x * C.x + C.y * C.y) * (B.x- A.x)) /d; 
        Vector2 center = new Vector2(ux,uy);
        float radius = Vector2.Distance(center, A);

        return new Circle(center, radius);
    }

    public bool ContainsVertex(Vector2 v)
    {
        return v == A || v == B || v == C;
    }

    public bool IsPointInCircumcircle(Vector2 point)
    {
        return Vector2.Distance(Circumcircle.Center, point) <= Circumcircle.Radius;
    }

}

public class Circle
{
    public Vector2 Center;
    public float Radius; 

    public Circle(Vector2 center, float radius)
    {
        this.Center = center;
        this.Radius= radius;
    }
}