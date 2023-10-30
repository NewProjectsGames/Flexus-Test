using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class TrajectoryController : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private bool viewReflect = false;
    private Vector3 _velocity;
    private Vector3 _position;
    private Vector3 _lastPosition;


    public void CalculationTrajectory(Transform _pointStart, float _power)
    {
        _velocity = _pointStart.forward * _power;
        lineRenderer.positionCount = 100;
        _position = _pointStart.position;
        _lastPosition = _position;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            _velocity.y -= Constant.Gravity * Time.fixedDeltaTime;
            _position += _velocity * Time.fixedDeltaTime;

            if (viewReflect)
            {
                if (DetectCollision(out var surfaceNormal, (_position - _lastPosition).normalized))
                {
         
                    if (surfaceNormal.z <= -.5f)
                    {
                        _velocity = Vector3.Reflect(_velocity, surfaceNormal);
                        _velocity *= Constant.Bounce/10f;
                    }else
                    {
                        _velocity = Vector3.Reflect(_velocity, surfaceNormal);
                        _velocity *= Constant.Bounce;
                    }
                }
            }

            _lastPosition = _position;
            lineRenderer.SetPosition(i, _position);
        }
    }

    private bool DetectCollision(out Vector3 surfaceNormal, Vector3 frw)
    {
        if (Physics.Raycast(_position, frw, out var hit, 1f, collisionLayer))
        {
            surfaceNormal = hit.normal;
            return true;
        }

        surfaceNormal = Vector3.up;
        return false;
    }
}