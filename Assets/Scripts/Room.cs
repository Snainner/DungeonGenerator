using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Room : MonoBehaviour
{
    BoxCollider _collider;
    public int _Index;
    public List<(int neighbour, float distance)> Links = new();
    public Bounds Bounds => _collider.bounds;
    //maybe add offset to bounds to prevent rooms from touching without colliding
    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();

    }

    public void SetSize(Vector3 size)
    {
        _collider.size = size;
    }

    public void CheckOverlap()
    {
        Collider[] overlaps = Physics.OverlapBox(_collider.bounds.center, _collider.bounds.extents, Quaternion.identity);
        foreach (var overlap in overlaps)
        {
            if (overlap != _collider)
            {
                Debug.Log($"Room {gameObject.name} overlaps with {overlap.gameObject.name}");
            }
        }
    }
}
