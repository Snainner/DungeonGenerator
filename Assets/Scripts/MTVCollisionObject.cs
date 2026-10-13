using Mono.Cecil;
using System;
using UnityEngine;

public class MTVCollisionObject : MonoBehaviour
{
    public LayerMask _obstacleLayer;

    public bool _autoResolve = true;
    public bool _smoothResolve = false;

    public event Action<Vector3> _OnPenetrationStart;
    public event Action<Vector3> _OnPenetrationStay;
    public event Action _OnPenetrationEnd;

    Collider _collider;
    Vector3 _lastCorrection;
    bool _resolvingCollision;

    int _roomLayer = 6;
    int _layerAsLayerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _collider = GetComponent<Collider>();
        
        _obstacleLayer = (1 << _roomLayer);
    }

    public void ResolveCollision()
    {
        bool isColliding = _collider.GetPenetrationInLayers(_obstacleLayer, out Vector3 correction);
        //Add small amount of offset to avoid values very close to zero
        correction += correction.normalized * 0.001f;
        _lastCorrection = isColliding? correction : Vector3.zero;   

        if(isColliding)
        {
            if (!_resolvingCollision)
                _OnPenetrationStart?.Invoke(correction);
            else
                _OnPenetrationStay?.Invoke(correction);

            _resolvingCollision = true;

            if(_autoResolve)
            {
                Vector3 delta = _smoothResolve ? Vector3.Lerp(Vector3.zero, correction, 0.1f) : correction;
                transform.position += delta;
            }
        }
        else
        {
            if (_resolvingCollision)
                _OnPenetrationEnd?.Invoke();
            _resolvingCollision= false;
        }
    }

    private void OnDrawGizmos()
    {
        if (_collider == null) return;

        if(_lastCorrection != Vector3.zero)
        {
            Vector3 start = _collider.bounds.center;
            Vector3 end = start + _lastCorrection;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawSphere(end, .05f);
        }
    }
}
