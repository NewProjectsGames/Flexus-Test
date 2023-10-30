using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    [SerializeField] private Transform cameraTransform;  
    [SerializeField] private float shakeDuration = 0.2f;  
    [SerializeField] private float shakeAmount = 0.2f; 

    private Vector3 _originalPosition; 
    private float _shakeTimer = 0f;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        _originalPosition = cameraTransform.localPosition;
    }

    void Update()
    {
        if (_shakeTimer > 0)
        {
            cameraTransform.localPosition = _originalPosition + Random.insideUnitSphere * shakeAmount;
            _shakeTimer -= Time.deltaTime;
        }
        else
        {
            _shakeTimer = 0f;
            cameraTransform.localPosition = _originalPosition;
        }
    }

    public void ShakeCamera()
    {
        _shakeTimer = shakeDuration;
    }
}