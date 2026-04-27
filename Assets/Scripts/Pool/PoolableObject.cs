using Flexus.Interfaces;
using UnityEngine;

namespace Flexus.Pool
{
    /// <summary>
    /// Component attached to any pooled prefab that needs to notify its pool
    /// when it is disabled externally (e.g. by another system).
    /// Implements the <b>IPoolable</b> contract so <see cref="ObjectPool{T}"/>
    /// can manage its lifecycle without knowing the concrete type.
    /// </summary>
    public class PoolableObject : MonoBehaviour, IPoolable
    {
        private bool _isDespawning;

        // ── IPoolable ────────────────────────────────────────────────────

        /// <inheritdoc/>
        public virtual void OnSpawn()
        {
            _isDespawning = false;
            // Subclasses may override to reset state on retrieval.
        }

        /// <inheritdoc/>
        public virtual void OnDespawn()
        {
            if (_isDespawning) return;
            _isDespawning = true;
            gameObject.SetActive(false);
        }

        // ── Unity lifecycle ──────────────────────────────────────────────

        /// <summary>
        /// Ensures the object is fully despawned if it gets disabled externally
        /// (e.g., the scene is unloaded or another system calls SetActive(false)).
        /// </summary>
        private void OnDisable() => OnDespawn();
    }
}
