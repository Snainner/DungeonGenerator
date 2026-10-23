using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class MSP : MonoBehaviour
{
    Dictionary<Edge, float> _edgeWeights = new Dictionary<Edge, float>();

    public static List<Edge> Prim(List<Edge> edges, Vector2 start)
    {
        List<Vector2> unVisited = new List<Vector2>();
        List<Vector2> visited = new List<Vector2>();

        foreach(Edge edge in edges)
        {
            unVisited.Add(edge.A);
            unVisited.Add(edge.B);
        }

        visited.Add(start); 
        List<Edge> results = new List<Edge>();

        while(visited.Count > 0)
        {
            bool chosen = false;
            Edge chosenEdge = null; 
            float minWeight = float.PositiveInfinity;

            foreach(Edge edge in edges)
            {
                int checkedVertices = 0;
                if(!visited.Contains(edge.A))
                    checkedVertices++;
                if(!visited.Contains(edge.B))
                    checkedVertices++;
                if(checkedVertices != 1) 
                    continue;
                
                if(edge.Distance() < minWeight)
                {
                    chosenEdge = edge;
                    chosen = true;
                    minWeight = edge.Distance();
                }
            }

            if(!chosen)
                break;
            
            results.Add(chosenEdge);
            unVisited.Remove(chosenEdge.A);
            unVisited.Remove(chosenEdge.B);
            visited.Add(chosenEdge.A);
            visited.Add(chosenEdge.B);

        }

        
        return results;
    }

    public static void AddRandomEdgesBack(List<Edge> allEdges, List<Edge> MSP, int amount)
    {
        int edgesAdded = 0;

        //Remove msp edges from all edges to avoid picking one that already exists 
        // foreach(Edge edge in MSP)
        // {
        //     allEdges.Remove(edge);
        // }
        Utils.ListUtils.Shuffle(allEdges);
        HashSet<Edge> edgeSet = new HashSet<Edge>();
        
        foreach(Edge edge in MSP)
        {
            edgeSet.Add(edge);
        }
        
        int i = 0;
        while(edgesAdded < amount && i < allEdges.Count)
        {
            if(edgeSet.Add(allEdges[i]))
            {
                MSP.Add(allEdges[i]);
                edgesAdded++;
            }
            i++;
        }

        Debug.Log($"Random edges added back {edgesAdded}");
    }

}
