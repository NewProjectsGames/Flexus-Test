using Flexus.Core;
using Flexus.Interfaces;
using UnityEngine;

namespace Flexus.Visual
{
    /// <summary>
    /// Renders a predicted projectile trajectory using a <see cref="LineRenderer"/>.
    /// Implements <see cref="ITrajectoryService"/> so the cannon controller depends only
    /// on the interface, never on this concrete MonoBehaviour.
    ///
    /// <para>
    /// Mirrors the same Euler-integration and bounce logic used in
    /// <see cref="Flexus.Projectile.Projectile"/> so the preview is physically accurate.
    /// </para>
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public sealed class TrajectoryController : MonoBehaviour, ITrajectoryService
    {
        // ── Inspector ────────────────────────────────────────────────────

        [Tooltip("Layer mask used when checking for surface collisions along the path.")]
        [SerializeField] private LayerMask collisionLayer;

        [Tooltip("Number of simulation steps rendered on the line (higher = longer preview).")]
        [SerializeField] private int simulationSteps = 100;

        [Tooltip("When enabled, the trajectory preview accounts for surface bounces.")]
        [SerializeField] private bool showReflections = false;

        // ── Private state ────────────────────────────────────────────────

        private LineRenderer _lineRenderer;

        // ── Unity lifecycle ──────────────────────────────────────────────

        private void Awake() => _lineRenderer = GetComponent<LineRenderer>();

        // ── ITrajectoryService ───────────────────────────────────────────

        /// <inheritdoc/>
        public void CalculateTrajectory(Transform startPoint, float power)
        {
            Vector3 velocity         = startPoint.forward * power;
            Vector3 position         = startPoint.position;
            Vector3 previousPosition = position;

            _lineRenderer.positionCount = simulationSteps;

            for (int step = 0; step < simulationSteps; step++)
            {
                // Integrate gravity and advance position.
                velocity.y -= GameConstants.Gravity * Time.fixedDeltaTime;
                position   += velocity * Time.fixedDeltaTime;

                if (showReflections)
                {
                    Vector3 direction = (position - previousPosition).normalized;
                    if (TryDetectCollision(position, direction, out Vector3 surfaceNormal))
                        velocity = GameConstants.CalculateBounceVelocity(velocity, surfaceNormal);
                }

                previousPosition = position;
                _lineRenderer.SetPosition(step, position);
            }
        }

        // ── Private helpers ──────────────────────────────────────────────

        /// <summary>Casts a ray in <paramref name="direction"/> to check for surface contact.</summary>
        private bool TryDetectCollision(Vector3 currentPos, Vector3 direction, out Vector3 surfaceNormal)
        {
            if (Physics.Raycast(currentPos, direction, out RaycastHit hit, 1f, collisionLayer))
            {
                surfaceNormal = hit.normal;
                return true;
            }

            surfaceNormal = Vector3.up;
            return false;
        }
    }
}
