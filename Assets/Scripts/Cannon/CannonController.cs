using Flexus.Camera;
using Flexus.Interfaces;
using Flexus.Managers;
using Flexus.Projectile;
using Flexus.Visual;
using ProjectileComponent = Flexus.Projectile.Projectile; // alias: namespace and class share the same short name
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Flexus.Cannon
{
    /// <summary>
    /// Handles player input for aiming and firing the cannon.
    /// All heavy lifting is delegated to injected services; this class
    /// is purely an input translator following the <b>Single Responsibility Principle</b>.
    ///
    /// <para>
    /// Dependency graph:
    /// <list type="bullet">
    ///   <item><description><see cref="IProjectilePool"/> — supplies ready-to-fire projectiles.</description></item>
    ///   <item><description><see cref="ITrajectoryService"/> — draws the predicted arc.</description></item>
    ///   <item><description><see cref="ICameraShakeService"/> — triggers camera feedback on fire.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class CannonController : MonoBehaviour
    {
        private static readonly int FireHash = Animator.StringToHash("Fire");
        // ── Inspector ────────────────────────────────────────────────────

        [Tooltip("Current launch power; also exposed to UI sliders via ChangePower().")]
        [SerializeField] private int power = 20;

        [Tooltip("Maximum upward barrel angle in degrees.")]
        [SerializeField] private float maxVerticalAngle = 45f;

        [Tooltip("Maximum downward barrel angle in degrees.")]
        [SerializeField] private float minVerticalAngle = -15f;

        [Tooltip("Degrees-per-second multiplier applied to mouse delta during aiming.")]
        [SerializeField] private float rotationSpeed = 100f;

        [Tooltip("Barrel pivot transform (child object that rotates vertically).")]
        [SerializeField] private Transform barrelPivot;

        [Tooltip("World-space spawn point for fired projectiles.")]
        [SerializeField] private Transform projectileSpawnPoint;

        // ── Injected services ────────────────────────────────────────────

        private IProjectilePool     _projectilePool;
        private ITrajectoryService  _trajectoryService;
        private ICameraShakeService _cameraShake;

        /// <summary>
        /// Zenject method injection; called after the object is resolved.
        /// </summary>
        [Inject]
        private void Construct(
            IProjectilePool     projectilePool,
            ITrajectoryService  trajectoryService,
            ICameraShakeService cameraShake)
        {
            _projectilePool    = projectilePool;
            _trajectoryService = trajectoryService;
            _cameraShake       = cameraShake;
        }

        // ── Private state ────────────────────────────────────────────────

        private Animator _animator;
        private Vector3  _lastMousePosition;
        private bool     _isDragging;

        // ── Unity lifecycle ──────────────────────────────────────────────

        private bool _isReady;

        private void Awake() => _animator = GetComponent<Animator>();

        private void Start()
        {
            if (_trajectoryService == null || _projectilePool == null || _cameraShake == null)
            {
                Debug.LogError(
                    $"[{nameof(CannonController)}] Dependencies are missing! Zenject did not inject them. " +
                    "Make sure your Scene has a GameObject with a 'SceneContext' component, and that " +
                    "'GameInstaller' is added to the SceneContext's Installers list.",
                    gameObject);
                enabled = false;
                return;
            }

            _isReady = true;
        }

        private void Update()     { if (_isReady) HandleInput(); }
        private void LateUpdate() { if (_isReady) _trajectoryService.CalculateTrajectory(projectileSpawnPoint, power); }

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Bound to a UI Slider's OnValueChanged event to update launch power.
        /// </summary>
        public void ChangePower(float value) => power = Mathf.RoundToInt(value);

        // ── Private helpers ──────────────────────────────────────────────

        /// <summary>
        /// Central input handler — guards UI-layer clicks, then delegates to
        /// <see cref="Aim"/> and <see cref="Fire"/>.
        /// </summary>
        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Prevent the cannon from reacting to clicks on UI elements.
                if (!EventSystem.current.IsPointerOverGameObject())
                    _isDragging = true;
            }

            if (_isDragging)
            {
                Aim();
                Fire();
            }

            if (Input.GetMouseButtonUp(0))
                _isDragging = false;
        }

        /// <summary>
        /// Translates mouse delta into horizontal (whole cannon) and vertical
        /// (barrel only) rotations while clamping the barrel angle.
        /// </summary>
        private void Aim()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _lastMousePosition = Input.mousePosition;
                return;
            }

            if (!Input.GetMouseButton(0)) return;

            Vector3 delta = Input.mousePosition - _lastMousePosition;
            _lastMousePosition = Input.mousePosition;

            // Horizontal — rotate the entire cannon base.
            float horizontalDelta = delta.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, horizontalDelta);

            // Vertical — rotate only the barrel pivot, clamped to inspector limits.
            float rawAngle = barrelPivot.localEulerAngles.x + (-delta.y * rotationSpeed * Time.deltaTime);

            // Remap from [0, 360] to [-180, 180] so Clamp works correctly.
            if (rawAngle > 180f) rawAngle -= 360f;
            float clampedAngle = Mathf.Clamp(rawAngle, minVerticalAngle, maxVerticalAngle);
            barrelPivot.localEulerAngles = new Vector3(clampedAngle, 0f, 0f);
        }

        /// <summary>
        /// Fires a projectile when the mouse button is released.
        /// Retrieves a pooled instance, positions and orients it, then calls Fire().
        /// </summary>
        private void Fire()
        {
            if (!Input.GetMouseButtonUp(0)) return;

            GameObject projectileGo = _projectilePool.GetProjectile();
            if (projectileGo == null)
            {
                Debug.LogWarning($"[{nameof(CannonController)}] ProjectilePool returned null — pool may be empty.");
                return;
            }

            projectileGo.SetActive(true);
            projectileGo.transform.SetParent(projectileSpawnPoint);
            projectileGo.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            if (projectileGo.TryGetComponent(out ProjectileComponent projectile))
                projectile.Fire(power);

            _animator.SetTrigger(FireHash);
            _cameraShake.ShakeCamera();
        }
    }
}
