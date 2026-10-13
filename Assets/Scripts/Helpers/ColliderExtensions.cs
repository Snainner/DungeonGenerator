using UnityEngine;

public static class ColliderExtensions 
{
    static readonly Collider[] _overlapCache = new Collider[32]; 

    public static bool GetPenetrationInLayers(this Collider source, LayerMask mask, out Vector3 totalCorrection)
    {
        totalCorrection = Vector3.zero;
        if (source == null)
            return false;

        //Fast check for overlaps
        int count = Physics.OverlapBoxNonAlloc(
            source.bounds.center,
            source.bounds.extents,
            _overlapCache,
            source.transform.rotation,
            mask);
        
        bool collided = false;
        for (int i = 0; i < count; i++)
        {
            Collider other = _overlapCache[i];
            if (other == source) continue;

            //If this comes back true it means there is actual collision happening and not just bounds intersection  
            if(source.ComputePenetration(other, out Vector3 dir, out float dist))
            {
                collided = true;
                totalCorrection += dir * dist;
            }
        }
        return collided;
    }

    //Find the minimum translation vector (MTV) between two colliders
    public static bool ComputePenetration(this Collider source, Collider target, out Vector3 direction, out float distance)
    {
        direction = Vector3.zero;
        distance = 0f;

        if(source == null || target == null) 
            return false;

        //If these two colliders are overlapping now, what is the smallest move to stop that
        return Physics.ComputePenetration(
            source, source.transform.position, source.transform.rotation,
            target, target.transform.position, target.transform.rotation,
            out direction, out distance);
    }
}
