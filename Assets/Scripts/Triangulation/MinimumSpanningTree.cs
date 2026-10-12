using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MinimumSpanningTree 
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="V">Vertice Count/ amount of nodes</param>
    /// <param name="edges"></param>
    /// <returns></returns>
    public static List<Edge> KruskalsMSP(int V, List<Edge> edges)
    {
        DisjointSet disjointSet = new DisjointSet();
        disjointSet.Initialize(V);
        float cost = 0;
        int count = 0;
        List<Edge> MSP = new List<Edge>();

        foreach (var e in edges)
        {
            //Check if there is no cycle
            if(disjointSet.Find(e.A) != disjointSet.Find(e.B))
            {
                disjointSet.Union(e.A, e.B);
                MSP.Add(e);
                cost += e.Distance;

                if (++count == V - 1)
                    break;
            }

        }
        Debug.Log($"Edge Count: {MSP.Count},Expected Count: {V-1} ,MSP Cost:{cost}");
        return MSP;
    }
    public static List<Edge> BuildAndSortEdges(int count, Func<int, int, float> distance)
    {
        var edges = BuildEdges(count, distance);
        var sortedEdges = SortEdges(edges);
        foreach( var edge in sortedEdges) 
            Debug.Log(edge.Distance);

        return sortedEdges;
    }
    public static List<Edge> BuildEdges(int count, Func<int, int, float> distance)
    {
        var edges = new List<Edge>();
        for (int a = 0; a < count; a++)
        {
            for (int b = a + 1; b < count; b++)
                edges.Add(new Edge { A = a, B = b, Distance = distance(a, b) });
        }
        return edges;
    }

    public static List<Edge> SortEdges(List<Edge> edges)
    {
        //Sort edges in a non-decreasing way
        //Sort from small to big 
        var sortedEdges = new List<Edge>();

        return edges.OrderBy(e => e.Distance).ToList();
    }

   
}
