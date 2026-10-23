using UnityEngine;

public class FrameLimiter : MonoBehaviour
{
    [SerializeField] int _targetFPS = 60; 
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = _targetFPS;
        Debug.LogWarning($"Framerate limit set to {_targetFPS}");

    }
    
}
