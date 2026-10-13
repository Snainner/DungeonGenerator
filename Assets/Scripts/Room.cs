using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Room : MonoBehaviour
{
    [ShowNonSerializedField]
    Vector3 _size;
    [ShowNonSerializedField]
    Vector3 _centreOffset;
    bool _initialized = false;

    BoxCollider _collider;
    MTVCollisionObject _mtvCollision;

    //public List<(int neighbour, float distance)> Links = new();
    public int _Index;
    [ShowNativeProperty]
    public Bounds Bounds
    {
        get
        {
            Debug.Assert(_initialized, $"{name} Bounds accessed before Init()", this);
            return new Bounds(transform.position + _centreOffset, _size);
        }
    }
    //maybe add offset to bounds to prevent rooms from touching without colliding
    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _mtvCollision = this.AddComponent<MTVCollisionObject>();
    }
 
    /// <param name="centreOffset">Only needed if the pivot isn't the centre</param>
    public void Init(Vector3 size, Vector3 centreOffset = default)
    {
        _size = size;
        _centreOffset = centreOffset;
        _initialized = true;
    }
    public void MTVCollisionCheck()
    {
        if (_mtvCollision == null)
        {
            Debug.Log("MTV Collision Component not found");
            return;
        }

        _mtvCollision.ResolveCollision();
    }

    public void SetSize(Vector3 size)
    {
        _collider.size = size;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0,1,0,.1f);
        Gizmos.DrawCube(Bounds.center, Bounds.size);
        
    }

}
