using UnityEngine;

[CreateAssetMenu()]
public class MTVCollisionSettings : ScriptableObject
{
    public LayerMask LayerMask;
    public bool AutoResolve = true;
    public bool SmoothResolve = true;
}
