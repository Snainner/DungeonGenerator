//using NUnit.Framework;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainTools;
public struct Triangle
{
    public Point PointA;
    public Point PointB;
    public Point PointC;

    public Triangle(Point a, Point b, Point c)
    {
        PointA = a;
        PointB = b;
        PointC = c;
    }
}
//public class Triangle
//{
//    public Vector2 _PointA, _PointB, _PointC;
//    public List<Edge> _Edges;
//    public List<Triangle> _Links;
//    public float _CircumcircleRadius;
//    public Vector2 _CircumcircleCenter;
//    public int _PoolIndex;

//    //Constructer
//    public Triangle(int index)
//    {
//        _PointA = new Vector2(0f, 0f);
//        _PointB = new Vector2(0f, 0f);
//        _PointC = new Vector2(0f, 0f);
//        _Edges = new List<Edge>();
//        _Links = new List<Triangle>();

//        _Edges.Add(new Edge(_PointA, _PointB));
//        _Links.Add(null);
//        _Edges.Add(new Edge(_PointB, _PointC));
//        _Links.Add(null);
//        _Edges.Add(new Edge(_PointC, _PointA));
//        _Links.Add(null);

//        _PoolIndex = index;
//    }
//    /// <summary>
//    /// Set points and edges, links will only be set if needed
//    /// </summary>
//    public void Populate(Vector2 A, Vector2 B, Vector2 C)
//    {
//        _PointA = A;
//        SortCCW(B, C);

//        CalculateCircumscribedCircle();

//        _Edges.Clear();
//        _Edges.Add(new Edge(_PointA, _PointB));
//        _Edges.Add(new Edge(_PointB, _PointC));
//        _Edges.Add(new Edge(_PointC, _PointA));

//        _Links[0] = null;
//        _Links[1] = null;
//        _Links[2] = null;
//    }

//    //The circular area that contains each point of the triangle
//    public void CalculateCircumscribedCircle()
//    {
//        //First calculate the midpoint of two out of the three edges of the triangle
//        Vector2 midAB= new Vector2((_PointA.x + _PointB.x) / 2, (_PointA.y + _PointB.y) / 2);
//        Vector2 midAC = new Vector2((_PointA.x + _PointC.x) / 2, (_PointA.y + _PointC.y) / 2);

//        //Calculate the slope of the two edges
//        //then get the negative reciprocal for the perpendicular (m)
//        //reciprocal means of a number or function is the value or expression of that results from from reversing the numerator and denominators places 
//        // for example the reciprocal of 2 is 1/2 and the reciprocal of 1/3 is 3
//        float slopeAB = (_PointB.y - _PointA.y) / (_PointB.x - _PointA.x);
//        float slopeAC = (_PointC.y - _PointA.y) / (_PointC.x - _PointA.x);

//        //Check if the slope is 0, if it is then the perpendicular line will be vertical and the x value of the circumcircle center will be the same as the midpoint of the edge
//        if (slopeAB == 0.0f)
//        {
//            _CircumcircleCenter.x = midAB.x;
//            float intercept = midAC.y - midAC.x * (-1 / slopeAC); // negative reciprocal of slopeAC
//            _CircumcircleCenter.y = (-1 / slopeAC) * _CircumcircleCenter.x + intercept;
//        }
//        else if(slopeAC == 0.0f)
//        {
//            _CircumcircleCenter.x = midAC.x;
//            float intercept = midAB.y - midAB.x * (-1 / slopeAB); // negative reciprocal of slopeAB
//            _CircumcircleCenter.y = (-1 / slopeAB) * _CircumcircleCenter.x + intercept;
//        }
//        else
//        {
//            //Find the y-intercept of the perpendicular lines using the point-slope form of a line equation 
//            //y = m·x + q 

//            float interceptAB = midAB.y + midAB.x / slopeAB; // negative reciprocal of slopeAB
//            float interceptAC = midAC.y + midAC.x / slopeAC;
//            //Now that the equations of the two perpendicular lines are known, find the intersection point of the two lines to get the circumcircle center
//            _CircumcircleCenter.x = (interceptAC - interceptAB) / (-1/slopeAB + 1 / slopeAC);
//            _CircumcircleCenter.y = -_CircumcircleCenter.x / slopeAB + interceptAB;
//        }
//        //Find the radius with a vector operation 
//        _CircumcircleRadius = (_PointA - _CircumcircleCenter).magnitude;
//    }

//    /// <summary>
//    /// Sorts the triangle's points in counter-clockwise order based on the provided points B and C.
//    /// </summary>
//    /// <param name="B"></param>
//    /// <param name="C"></param>
//    public void SortCCW(Vector2 B, Vector2 C)
//    {
//        //find the "front facing" direction with the cross product sign
//        if(Vector3.Cross(_PointA - B, _PointA -C ).z < 0)
//        {
//            _PointB = C;
//            _PointC = B;
//        }
//        else
//        {
//            _PointB = B;
//            _PointC = C;
//        }
//    }

//    public bool IsPointInCircumcircle(Vector2 point)
//    {
//        return (point - _CircumcircleCenter).magnitude < _CircumcircleRadius;
//    }
//}
