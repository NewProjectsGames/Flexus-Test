using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    public float power = 10.0f;
    [SerializeField] private LayerMask collisionLayer;

    private Vector3 _velocity;
    private Vector3 _position;


    private void FixedUpdate()
    {
        _velocity.y -= Constant.Gravity * Time.fixedDeltaTime;
        _position += _velocity * Time.fixedDeltaTime;
        transform.LookAt(_position);
        transform.position = _position;
        if (DetectCollision(out var surfaceNormal))
        {
            if (surfaceNormal.z <= -.5f)
            {
                _velocity = Vector3.Reflect(_velocity, surfaceNormal);
                _velocity *= Constant.Bounce / 5f;
            }
            else
            {
                _velocity = Vector3.Reflect(_velocity, surfaceNormal);
                _velocity *= Constant.Bounce;
            }
        }
    }

    private bool DetectCollision(out Vector3 surfaceNormal)
    {
        if (Physics.Raycast(_position, transform.forward, out var hit, 1f, collisionLayer))
        {
            surfaceNormal = hit.normal;
            if (hit.transform.tag == "Wall")
            {
                if (_velocity.sqrMagnitude > 0.2f)
                    DecalPainter.Instance?.PaintDecal(hit);
            }

            if (_velocity.sqrMagnitude <= 0.1f)
            {
                Explosion(hit);
            }

            return true;
        }

        surfaceNormal = Vector3.up;
        return false;
    }

    private void Explosion(RaycastHit hit)
    {
        FxManager.Instance.Explosion(transform.position, hit.normal);
        gameObject.SetActive(false);
    }

    public void Fire(float _power)
    {
        transform.SetParent(null);
        power = _power;
        _velocity = transform.forward * power;
        _position = transform.position;
    }
}