using System.Collections.Generic;
using Flexus.Interfaces;
using Flexus.Projectile;
using UnityEngine;
using Zenject;

namespace Flexus.Managers
{
    /// <summary>
    /// Manages a pre-built pool of projectile <see cref="GameObject"/>s and implements
    /// <see cref="IProjectilePool"/> so consumers are decoupled from this concrete class.
    ///
    /// <para>
    /// Uses <see cref="ProjectileFactory"/> (Factory Method) to build each projectile,
    /// and a simple inactive-first search to return the next available instance.
    /// </para>
    /// </summary>
    public sealed class ProjectileManager : MonoBehaviour, IProjectilePool
    {
        // ── Inspector ────────────────────────────────────────────────────

        [Tooltip("Base prefab that carries the Projectile component. Mesh is added at runtime.")]
        [SerializeField] private GameObject prefabBaseProjectile;

        [Tooltip("Material applied to every generated projectile mesh.")]
        [SerializeField] private Material projectileMaterial;

        [Tooltip("How many projectile instances are pre-built on Start.")]
        [SerializeField] private int poolSize = 10;

        // ── Private state ────────────────────────────────────────────────

        private readonly List<GameObject> _pool = new();
        private DiContainer _container;

        [Inject]
        private void Construct(DiContainer container)
        {
            _container = container;
        }

        // ── Unity lifecycle ──────────────────────────────────────────────

        private void Start() => PreWarm();

        // ── IProjectilePool ──────────────────────────────────────────────

        /// <inheritdoc/>
        /// <remarks>
        /// Returns the first inactive projectile. Falls back to the first element
        /// in the pool if all are active (avoids runtime allocations).
        /// </remarks>
        public GameObject GetProjectile()
        {
            GameObject selected = null;

            for (int i = 0; i < _pool.Count; i++)
            {
                if (!_pool[i].activeInHierarchy)
                {
                    selected = _pool[i];
                    _pool.RemoveAt(i);
                    break;
                }
            }

            if (selected == null)
            {
                // Graceful fallback — recycle the oldest active projectile (at the front of the queue).
                Debug.LogWarning("[ProjectileManager] Pool exhausted; recycling oldest projectile.");
                selected = _pool[0];
                _pool.RemoveAt(0);
            }

            // Move the selected projectile to the end of the list so that
            // the start of the list always contains the oldest projectiles.
            _pool.Add(selected);
            return selected;
        }

        // ── Private helpers ──────────────────────────────────────────────

        /// <summary>Creates all pool instances using the <see cref="ProjectileFactory"/>.</summary>
        private void PreWarm()
        {
            _pool.Clear();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject instance = ProjectileFactory.Create(_container, prefabBaseProjectile, projectileMaterial);
                _pool.Add(instance);
            }
        }
    }
}
