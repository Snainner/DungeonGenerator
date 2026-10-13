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
        List<(Room a, Room b)> overlaps = FindOverlaps(rooms);
        if(overlaps.Count ==  0) return false;

        foreach (var overlap in overlaps)
        {
            Vector3 direction = (overlap.b.transform.position - overlap.a.transform.position).normalized;
            overlap.a.transform.position -= direction * pushDistance;
            overlap.b.transform.position += direction * pushDistance;
        }
        return true;
    }
    static List<(Room a, Room b)> FindOverlaps(List<Room> rooms)
    {
        List<(Room, Room)> overlaps = new List<(Room, Room)>();

        //Sort rooms from left to right based on their x position

        //rooms.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
        var entries = rooms.Select(r => (room: r, bounds: r.Bounds)).OrderBy(e=> e.bounds.min.x).ToList();

        for (int i = 0; i < rooms.Count; i++)
        {
            Bounds a = entries[i].bounds;

            for (int j = i + 1; j < entries.Count; j++)
            {
                //B starts at or after A's right edge, so we can break the loop early as the rooms are sorted by x position and no further rooms can overlap with A
                Bounds b = entries[j].bounds;
                if(b.min.x >= a.max.x)
                {
                    // Since the rooms are sorted, we can break early
                    Debug.Log("Broke the loop early");
                    break;
                }
                if(a.Intersects(b))
                {
                    overlaps.Add((entries[i].room, entries[j].room));
                    Debug.Log($"Overlap found between {entries[i].room.name} and {entries[j].room.name}");
                }
            }
        }
        return overlaps;
    }
}
