using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateEdges : MonoBehaviour
{
    [SerializeField] TrailRenderer _trailRenderer;
    [SerializeField] float _duration = .5f;
    [SerializeField] float _waitTime = 1f;

    private void Awake()
    {
        if (_trailRenderer == null)
            Debug.Log("Trail renderer not found on object");
    }

    public IEnumerator AnimateMSP(List<Edge> edges, List<Vector3> locations)
    {
        _trailRenderer.Clear(); 
        _trailRenderer.transform.position = locations[0];
        
        foreach (var e in edges)
        {
            Vector3 start = locations[e.A];
            Vector3 end = locations[e.B];
            Debug.DrawLine(start, end, Color.darkViolet, 1000f, false);

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / _duration;
                _trailRenderer.transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }
            yield return new WaitForSeconds(_waitTime);
        }

    }
}
