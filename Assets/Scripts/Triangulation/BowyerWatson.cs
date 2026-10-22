using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public struct Circumcircle
{
    public Vector3 Centre;
    public float Radius;

    public Circumcircle(Vector3 centre, float radius)
    {
        Centre = centre;
        Radius = radius;
    }
}

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
        //Create super triangle
        //Select a point and look for triangles whose circumcircle contains this point
        //Move the found triangles to the bad triangle list
        //Connect the bad triangles edges to the current point to make a new set of triangles
        //repeat

        for (int i = 0; i < points.Count; i++)
        {
            //check circumcircle 

        }

    }

    public static Circumcircle CalculateCircumcircle(Vector3 A, Vector3 B, Vector3 C)
    {
        Vector3 circumCenter = Vector3.zero;

        Vector3 midpointAB = new Vector3(((A.x - B.x) / 2),1, ((A.z - B.z) / 2));
        Vector3 midpointAC = new Vector3(((A.x - C.x) / 2), 1, ((A.z - C.z) / 2));

        float slopeAB = (B.z - A.z)/(B.x - A.x); //if the perpendicular bisector has a slope of -1/m the slope is perpendicular 
        float slopeAC = (C.z - A.z)/(C.x - A.x);

        if (slopeAB == 0.0f)
        {
            circumCenter.x = midpointAB.x;
            float intercept = midpointAC.z - midpointAC.x * (-1 / slopeAC);
            circumCenter.z = (-1 / slopeAC) * circumCenter.x + intercept;
        }
        else if (slopeAC == 0.0f)
        {
            circumCenter.x = midpointAC.x;
            float intercept = midpointAB.z - midpointAC.x * (-1 / slopeAB);
            circumCenter.y = (-1/slopeAB) * circumCenter.x + intercept;
        }
        else
        {
            //z = m * x + q
            float interceptAB = midpointAB.z + midpointAB.x / slopeAB;
            float interceptAC = midpointAC.z + midpointAC.x / slopeAC;

            circumCenter.x = (interceptAC - interceptAB) / (-1/slopeAB + 1/slopeAC);
            circumCenter.z = circumCenter.x / slopeAB + interceptAB;
        }

        float circumRadius = (A - circumCenter).magnitude;
        return  new Circumcircle(circumCenter, circumRadius);
    }

    public static bool IsInCircumcircle(Vector3 point, Circumcircle circle)
    {
        return (point - circle.Centre).magnitude < circle.Radius;
    }
}
