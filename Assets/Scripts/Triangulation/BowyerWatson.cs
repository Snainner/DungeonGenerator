using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using UnityEngine;


public static class BowyerWatson
{
    public static Triangle SuperTriangle(List<Point> points)
    {

        float maxX = float.MaxValue;
        float maxZ = float.MaxValue;
        float maxY = 0;

        float minX = float.MinValue;
        float minZ = float.MinValue;
        float minY = 0;

        foreach (Point p in points)
        {
            minX = Mathf.Min(minX, p.Location.x);
            minZ = Mathf.Min(minZ, p.Location.z);
            maxX = Mathf.Max(maxX, p.Location.x);
            maxZ = Mathf.Max(maxZ, p.Location.z);
        }
        Point superPointA = new Point(new Vector3(minX - 1, minY, minZ - 1));
        Point superPointB = new Point(new Vector3(maxX + 1, maxY, minZ - 1));
        Point superPointC = new Point(new Vector3((minX + maxX) / 2, maxY, maxZ + 1));

        return new Triangle(superPointA, superPointB, superPointC);
    }

    public static Triangle SuperTriangle(List<Vector3> points)
    {
        int padding = 30;

        float maxX = float.MinValue;
        float maxZ = float.MinValue;
        float maxY = 0;

        float minX = float.MaxValue;
        float minZ = float.MaxValue;
        float minY = 0;

        foreach (var p in points)
        {
            minX = Mathf.Min(minX, p.x);
            minZ = Mathf.Min(minZ, p.z);
            maxX = Mathf.Max(maxX, p.x);
            maxZ = Mathf.Max(maxZ, p.z);
        }
        Point superPointA = new Point(new Vector3(minX - padding, minY, minZ - padding));
        Point superPointB = new Point(new Vector3(maxX + padding, maxY, minZ - padding));
        Point superPointC = new Point(new Vector3((minX + maxX) / 2, maxY, maxZ + padding));

        return new Triangle(superPointA, superPointB, superPointC);
    }

    public static void Triangulate(List<Vector3> points)
    {
        Triangle superTriangle = SuperTriangle(points);
        List<Triangle> trackedTriangles = new List<Triangle>() { superTriangle};
        List<Triangle> badTriangles = new List<Triangle>();
        
        
        //Create super triangle
        //Select a point and look for triangles whose circumcircle contains this point
        //Move the found triangles to the bad triangle list
        //Connect the bad triangles edges to the current point to make a new set of triangles
        //repeat

        for (int i = 0; i < points.Count; i++)
        {
            //check circumcircle 
            var newTriangle = new VTriangle(points[1], superTriangle.PointB.Location, superTriangle.PointC.Location);


        }

    }

    


}
