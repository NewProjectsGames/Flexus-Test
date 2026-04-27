using System;
using Flexus.Core;
using Flexus.Interfaces;
using Flexus.Managers;
using Flexus.Pool;
using Flexus.Visual;
using UnityEngine;
using Zenject;

namespace Flexus.Projectile
{
    /// <summary>
    /// Simulates ballistic motion with surface bouncing and collision detection.
    /// Relies on injected services for visual feedback (FX, decals) so it has
    /// zero dependency on concrete singleton classes.
    ///
    /// <para><b>Patterns used:</b> Dependency Injection (Zenject), Observer (C# event).</para>
    /// </summary>
     public class Projectile : PoolableObject
    {
        // ── Inspector ────────────────────────────────────────────────────

        [Tooltip("Layer mask that defines collidable surfaces.")]
        [SerializeField] private LayerMask collisionLayer;

        [Tooltip("Maximum number of surface bounces before the projectile explodes.")]
        [SerializeField] private int maxBounces = 3;

        // ── Events ───────────────────────────────────────────────────────

        /// <summary>
        /// Raised immediately before the projectile is deactivated after an explosion.
        /// Subscribers receive the raycast hit for contextual data (position, normal, etc.).
        /// </summary>
        public event Action<RaycastHit> OnExplosion;

        // ── Injected services ────────────────────────────────────────────

        private IFxService _fxService;
        private IDecalService _decalService;

        /// <summary>Zenject method injection — called after the object is created.</summary>
        [Inject]
        private void Construct(IFxService fxService, IDecalService decalService)
        {
            _fxService   = fxService;
            _decalService = decalService;
        }

        // ── Private state ────────────────────────────────────────────────

        private Vector3 _velocity;
        private Vector3 _position;
        private int     _remainingBounces;

        // ── Unity lifecycle ──────────────────────────────────────────────

        private void Awake()
        {
            if (_fxService == null)
                Debug.LogWarning($"[{nameof(Projectile)}] IFxService not injected — explosions will be silent.", gameObject);
            if (_decalService == null)
                Debug.LogWarning($"[{nameof(Projectile)}] IDecalService not injected — decals will be skipped.", gameObject);
        }

        // ── IPoolable ────────────────────────────────────────────────────

        /// <inheritdoc/>
        public override void OnSpawn()
        {
            if (TryGetComponent(out MeshFilter filter) && filter.sharedMesh != null)
            {
                // Jitter vertices on each spawn to ensure true randomness
                // even when projectiles are reused from the object pool.
                ProjectileFactory.RandomizeMesh(filter.sharedMesh, 0.9f);
            }

        }

        /// <inheritdoc/>
        public override void OnDespawn()
        {
            OnExplosion = null; // Clear listeners to avoid memory leaks across pool cycles.
            base.OnDespawn();
        }

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Detaches the projectile from its launch point and initiates ballistic motion.
        /// </summary>
        /// <param name="power">Launch speed scalar applied to the forward direction.</param>
        public void Fire(float power)
        {
            transform.SetParent(null);
            _remainingBounces = maxBounces;
            _velocity         = transform.forward * power;
            _position         = transform.position;
        }

        // ── Unity lifecycle ──────────────────────────────────────────────

        private void FixedUpdate()
        {
            // Euler integration with gravity.
            _velocity.y -= GameConstants.Gravity * Time.fixedDeltaTime;
            _position   += _velocity * Time.fixedDeltaTime;

            transform.LookAt(_position);
            transform.position = _position;

            if (TryDetectCollision(out RaycastHit hit))
                HandleCollision(hit);
        }

        // ── Private helpers ──────────────────────────────────────────────

        /// <summary>
        /// Casts a short ray forward to detect imminent surface contact.
        /// </summary>
        private bool TryDetectCollision(out RaycastHit hit)
        {
            if (_velocity.sqrMagnitude < 0.001f)
            {
                hit = default;
                return false;
            }
            return Physics.Raycast(_position, _velocity.normalized, out hit, 1f, collisionLayer);
        }

        /// <summary>
        /// Applies decals on walls, reflects or explodes the projectile depending on
        /// remaining bounces and current velocity.
        /// </summary>
        private void HandleCollision(RaycastHit hit)
        {
            if (hit.transform.CompareTag(GameConstants.WallTag)
                && _velocity.sqrMagnitude > GameConstants.MinVelocitySqr)
            {
                _decalService.PaintDecal(hit);
            }

            _remainingBounces--;

            bool shouldExplode = _remainingBounces <= 0
                              || _velocity.sqrMagnitude <= GameConstants.MinVelocitySqr;

            if (shouldExplode)
            {
                Explode(hit);
                return;
            }

            // Reflect velocity and apply extra damping.
            _velocity = GameConstants.CalculateBounceVelocity(_velocity, hit.normal);
        }

        /// <summary>
        /// Triggers the explosion effect, raises the event, and returns to pool.
        /// </summary>
        private void Explode(RaycastHit hit)
        {
            _fxService?.PlayExplosion(transform.position, hit.normal);
            OnExplosion?.Invoke(hit);
            gameObject.SetActive(false);
        }
    }
}
