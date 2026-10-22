using UnityEngine;

//Maybe remove this as it is only acting as a wrapper for a single vector3
public struct Point
{
    public Vector3 Location;

    public Point(Vector3 Location)
    {
        this.Location = Location;
    }
}
