using Flexus.Interfaces;
using UnityEngine;

namespace Flexus.Camera
{
    /// <summary>
    /// Applies a positional shake impulse to the camera rig whenever
    /// <see cref="ShakeCamera"/> is called.
    ///
    /// <para>
    /// Implements <see cref="ICameraShakeService"/> and is bound as a single
    /// instance via Zenject so no caller ever needs a static reference.
    /// </para>
    /// </summary>
    public sealed class CameraShake : MonoBehaviour, ICameraShakeService
    {
        // ── Inspector ────────────────────────────────────────────────────

        [Tooltip("Transform of the camera to shake (child of this GameObject).")]
        [SerializeField] private Transform cameraTransform;

        [Tooltip("Duration of a single shake impulse in seconds.")]
        [SerializeField] private float shakeDuration = 0.2f;

        [Tooltip("Maximum displacement radius during a shake in world units.")]
        [SerializeField] private float shakeRadius = 0.2f;

        // ── Private state ────────────────────────────────────────────────

        private Vector3 _restPosition;
        private float   _remainingTime;

        // ── Unity lifecycle ──────────────────────────────────────────────

        private void Start() => _restPosition = cameraTransform.localPosition;

        private void Update()
        {
            if (_remainingTime <= 0f) return;

            // Shake intensity scales linearly with remaining time (eases out).
            float intensity = _remainingTime / shakeDuration;
            cameraTransform.localPosition = _restPosition
                + Random.insideUnitSphere * (shakeRadius * intensity);

            _remainingTime -= Time.deltaTime;
            
            if (_remainingTime <= 0f)
                cameraTransform.localPosition = _restPosition;
        }

        // ── ICameraShakeService ──────────────────────────────────────────

        /// <inheritdoc/>
        public void ShakeCamera() => _remainingTime = shakeDuration;
    }
}
