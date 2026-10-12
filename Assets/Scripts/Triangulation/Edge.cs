using UnityEngine;

public struct Edge
{
    public int A, B;
    public float Distance;
    public Edge(int a, int b, float distance)
    {
        A = a;
        B = b;
        Distance = distance;
    }

    public bool CompareEdge(Edge edge)
    {
        return (A == edge.A && B == edge.B) || (A == edge.B && B == edge.A);
    }
}