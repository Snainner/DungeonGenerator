using UnityEngine;
using System.Collections.Generic;
using System;
using System.Numerics;
using UnityEditor.SceneManagement;

public class PathFinding2D : MonoBehaviour
{
    public class Node
    {
        public Vector2Int Position;
        public Node Previous;
        public float Cost;

        public Node(Vector2Int position)
        {
            Position = position;
        }
    }
    
   
    public struct PathCost
    {
        public bool traversable; 
        public float cost; 
    }

    static readonly Vector2Int[] neighbours =
    {
        new Vector2Int(1,0),
        new Vector2Int(0,1),
        new Vector2Int(-1,0),
        new Vector2Int(0,-1)
    };

    Grid2D<Node> grid;
    Utils.PriorityQueue<Node, float> queue; 
    HashSet<Node> closed;
    Stack<Vector2Int> stack; 

    public PathFinding2D(Vector2Int size)
    {
        grid = new Grid2D<Node>(size, Vector2Int.zero);

        queue = new Utils.PriorityQueue<Node, float>();
        closed = new HashSet<Node>();
        stack = new Stack<Vector2Int>();

        for (int x = 0; x < size.x; x++)
        {
            for (int y =0; y < size.y; y++)
            {
                grid[x,y] = new Node(new Vector2Int(x,y));
            }
        }
    }

    void ResetNodes()
    {
        var size = grid.Size;

        for (int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < size.y; y++)
            {
                var node = grid[x,y];
                node.Previous = null;
                node.Cost = float.PositiveInfinity;
            }
        }
    }
        
    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, Func<Node, Node, PathCost> costFunction)
    {
        ResetNodes();
        queue.Clear();
        closed.Clear(); 

        queue = new Utils.PriorityQueue<Node, float>();
        closed = new HashSet<Node>();

        grid[start].Cost = 0;
        queue.Enqueue(grid[start], 0);

        while(queue.Count > 0)
        {
            Node node = queue.Dequeue();
            closed.Add(node);
        
            if(node.Position == end)
            {
                ReconstructPath(node);
            }
      
            foreach(var offset in neighbours)
            {
                if(!grid.InBounds(node.Position + offset))
                    continue;
                
                var neighbour = grid[node.Position + offset];
                if(closed.Contains(neighbour))
                    continue;

                var pathCost = costFunction(node, neighbour);
                if(!pathCost.traversable)
                    continue;
                
                float newCost = node.Cost + pathCost.cost;
                if(newCost < neighbour.Cost)
                {
                    neighbour.Previous = node;
                    neighbour.Cost = newCost; 

                    if(queue.TryGetPriority( node ,out float existingPriority))
                    {
                        queue.UpdatePriority(node, newCost);
                    }
                    else
                    {
                        queue.Enqueue(neighbour, neighbour.Cost);
                    }
                }
            }
        }

        return null; 
    }

    List<Vector2Int> ReconstructPath(Node node)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        //This loop builds the path by walking backwards from the goal to the start 
        while (node != null)
        {
            stack.Push(node.Position);
            node = node.Previous;
        }

        //This loop reverses the order
        //Stacks are ordered by last in first out so by using pop to get the last result adds it to the queue
        // and a queue is first in first out which makes the result go from the start to the goal
        while (stack.Count > 0)
        {
            result.Add(stack.Pop());
        }

        return result;
    }
}
  

