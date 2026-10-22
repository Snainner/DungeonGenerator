using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class GenerationManager : MonoBehaviour
{
    RandomRoomPlacer2D _roomGenerator2D;
    GenerationData2D _generationData2D;
    TriangulationManager _triangulationManager;
    LineRenderer _lineRenderer;
    AnimateEdges _animEdges;

    List<Edge> _MSP = new List<Edge>();
    Coroutine _coroutine;
    private void Start()
    {
        _roomGenerator2D = GetComponent<RandomRoomPlacer2D>();
        if (_roomGenerator2D == null)
            Debug.Log("Room Gen component not found on generation manager object");

        _triangulationManager = GetComponent<TriangulationManager>();
        if (_triangulationManager == null)
            Debug.Log("Triangulation Manager not found on generation manager object");

        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer == null)
            Debug.Log("Line renderer not found on object");
        _animEdges = GetComponent<AnimateEdges>();
        if (_animEdges == null)
            Debug.Log("Animate Edges component not found on object");
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    void GenerateRooms2D()
    {
        _roomGenerator2D.Generate();
        Debug.Log($"Room Count:{_roomGenerator2D.rooms.Count}");
        _generationData2D = _roomGenerator2D.generationData;
    }
    
    //[Button(enabledMode: EButtonEnableMode.Playmode)]
    void CheckForOverlapMTV()
    {
        foreach(Room room in _roomGenerator2D.rooms)
            room.MTVCollisionCheck();
        //if (!_roomGenerator2D.CheckOverlapping())
        //    return;
        //else
        //    CheckForOverlap();
    }
    
    [Button(enabledMode: EButtonEnableMode.Playmode)]
    void CreateMSP()
    {
        int roomCount = _generationData2D.rooms.Count;
        var locations = _generationData2D.locations;

        var sortedEdges = MinimumSpanningTree.BuildAndSortEdges(roomCount, (a, b) => Vector3.Distance(locations[a], locations[b]));
        var krukalMSP = MinimumSpanningTree.KruskalsMSP(roomCount, sortedEdges);
        _MSP = krukalMSP;

        foreach (Edge edges in krukalMSP)
            Debug.DrawLine(locations[edges.A], locations[edges.B], Color.blue, 100f, false);

        foreach (Edge edges in sortedEdges)
            Debug.DrawLine(locations[edges.A], locations[edges.B], new Color(1f, 1f, 1f, 0.1f), 5f);
        //StartCoroutine(AnimateMSP(krukalMSP));
        //StartCoroutine(_animEdges.AnimateMSP(krukalMSP, locations));
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    void RestartTrailRenderer()
    {
        if(_coroutine !=  null)
        {
            _coroutine = StartCoroutine(_animEdges.AnimateMSP(_MSP, _generationData2D.locations));
        }
        else
        {
            StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(_animEdges.AnimateMSP(_MSP, _generationData2D.locations));
        }
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    void ResetGeneration()
    {
        _roomGenerator2D.Regen();
    }

    IEnumerator LineRendererAnimateMSP(List<Edge> MSP)
    {
        var locations = _generationData2D.locations;
        int positionCount = 2;
        _lineRenderer.positionCount = positionCount;
        _lineRenderer.SetPosition(0, locations[0]);
        _lineRenderer.SetPosition(1, locations[1]);

        float speed = 10f;
        int startPoint = 0, endPoint = 1;
        
        //_lineRenderer.positionCount = MSP.Count *2;
        foreach (var e in MSP)
        {
            Vector3 start = locations[e.A];
            Vector3 end = locations[e.B];

            _lineRenderer.SetPosition(startPoint, start);
            _lineRenderer.SetPosition(endPoint, start);
            float t = 0f;
            while (t < 5f)
            {
                t += Time.deltaTime / speed;
                _lineRenderer.SetPosition(endPoint,Vector3.Lerp(start, end, t));
               
            }
            _lineRenderer.SetPosition(endPoint, end);
            startPoint++;
            endPoint++;
            _lineRenderer.positionCount += 2;
            yield return new WaitForSeconds(1f);
        }
            

    }
}
