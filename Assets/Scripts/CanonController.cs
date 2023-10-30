using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CanonController : MonoBehaviour
{
    public int power;
    public float maxVerticalAngle = 45.0f;
    public float minVerticalAngle = -15.0f;
    [SerializeField] private float speedRotation;
    [SerializeField] private Transform rotationCanon;
    [SerializeField] private Transform pointStartProjectile;
    private Vector3 _lastMousePosition;
    private bool _isDragging;
    private ProjectileManager _projectileManager;
    private TrajectoryController _trajectoryController;
    private Animator _animator;

    // Start is called before the first frame update
    private void Start()
    {
        _projectileManager = FindObjectOfType<ProjectileManager>();
        _trajectoryController = FindObjectOfType<TrajectoryController>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        Control();
    }

    public void ChangePower(float value)
    {
        power = (int)value;
    }


    private void Aim()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = Input.mousePosition;

            Vector3 mouseDelta = currentMousePosition - _lastMousePosition;

            var horizontalRotation = mouseDelta.x * speedRotation * Time.deltaTime;
            transform.Rotate(Vector3.up, horizontalRotation);

            var verticalRotation = -mouseDelta.y * speedRotation * Time.deltaTime;
            var newVerticalAngle = rotationCanon.localEulerAngles.x + verticalRotation;


            if (newVerticalAngle > 180f)
                newVerticalAngle -= 360f;

            newVerticalAngle = Mathf.Clamp(newVerticalAngle, minVerticalAngle, maxVerticalAngle);
            rotationCanon.localEulerAngles = new Vector3(newVerticalAngle, 0, 0);

            _lastMousePosition = currentMousePosition;
        }
    }

    private void LateUpdate()
    {
        _trajectoryController.CalculationTrajectory(pointStartProjectile, power);
    }

    private void Fire()
    {
        if (Input.GetMouseButtonUp(0))
        {
            GameObject gm = _projectileManager.GetProjectile();
            gm.SetActive(true);
            gm.transform.SetParent(pointStartProjectile);
            gm.transform.localPosition = Vector3.zero;
            gm.transform.localEulerAngles = Vector3.zero;
            gm.GetComponent<Projectile>().Fire(power);
            _animator.SetTrigger("Fire");
            CameraShake.Instance?.ShakeCamera();
        }
    }


    private void Control()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                _isDragging = true;
            }
        }

        if (_isDragging)
        {
            Aim();
            Fire();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }
    }
}