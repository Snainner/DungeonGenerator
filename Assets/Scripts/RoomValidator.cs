using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RoomValidator 
{
    /// <summary>
    /// FIX: Needs an extra check after moving the overlapping rooms 
    /// </summary>
    public static bool PushRoomsApart(List<Room> rooms, float pushDistance = 1f)
    {
        bool anyOverlap = false;
        List<(Room a, Room b)> overlaps = FindOverlaps(rooms);
        List<Room> flatOverlapList = new List<Room>();
        foreach (var overlap in overlaps)
        {


            anyOverlap = true;
            Vector3 direction = (overlap.b.transform.position - overlap.a.transform.position).normalized;
            overlap.a.transform.position -= direction * pushDistance;
            overlap.b.transform.position += direction * pushDistance;

            //flatOverlapList.Add(overlap.a);
            //flatOverlapList.Add(overlap.b);
            //int overlapCheckCount = flatOverlapList.Count();
            //if (overlapCheckCount > 1)
            //{
            //    PushRoomsApart(flatOverlapList, pushDistance);
            //}

        }
        return anyOverlap;
    }

    static List<(Room a, Room b)> FindOverlaps(List<Room> rooms)
    {
        List<(Room, Room)> overlaps = new List<(Room, Room)>();

        //Sort rooms from left to right based on their x position

        rooms.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        for (int i = 0; i < rooms.Count; i++)
        {
            Bounds a = rooms[i].Bounds;

            for (int j = i + 1; j < rooms.Count; j++)
            {
                //B starts at or after A's right edge, so we can break the loop early as the rooms are sorted by x position and no further rooms can overlap with A
                Bounds b = rooms[j].Bounds;
                if(b.min.x >= a.max.x)
                {
                    // Since the rooms are sorted, we can break early
                    break;
                }
                if(a.Intersects(b))
                {
                    overlaps.Add((rooms[i], rooms[j]));
                    Debug.Log($"Overlap found between {rooms[i].name} and {rooms[j].name}");
                }
            }
        }
        return overlaps;
    }
}
