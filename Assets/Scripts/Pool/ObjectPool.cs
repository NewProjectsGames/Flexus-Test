using System.Collections.Generic;
using Flexus.Interfaces;
using UnityEngine;

namespace Flexus.Pool
{
    /// <summary>
    /// Generic, reusable object pool for Unity <see cref="MonoBehaviour"/> components.
    /// Pre-warms a fixed set of instances on <c>Awake</c> and returns the first
    /// inactive one on <see cref="Get"/>. Implements the <b>Object Pool</b> pattern.
    /// </summary>
    /// <typeparam name="T">
    /// Component type to pool; must implement <see cref="IPoolable"/>.
    /// </typeparam>
    public class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour, IPoolable
    {
        [Tooltip("Prefab to instantiate during pre-warm.")]
        [SerializeField] private T prefab;

        [Tooltip("Number of instances created on Awake.")]
        [SerializeField] private int initialSize = 10;

        private readonly List<T> _pool = new();

        // ── Unity lifecycle ──────────────────────────────────────────────

        private void Awake() => PreWarm();

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Returns an inactive pooled instance. If all instances are active,
        /// falls back to the first element (oldest shot wins).
        /// </summary>
        public T Get()
        {
            T instance = null;
            foreach (T item in _pool)
            {
                if (!item.gameObject.activeInHierarchy)
                {
                    instance = item;
                    break;
                }
            }

            if (instance == null)
            {
                // Fallback: recycle the oldest instance rather than allocating.
                instance = _pool[0];
                instance.OnDespawn();
            }

            instance.OnSpawn();
            return instance;
        }

        /// <summary>Deactivates and notifies <paramref name="item"/> it was returned.</summary>
        public void Return(T item)
        {
            item.OnDespawn();
            item.gameObject.SetActive(false);
        }

        // ── Private helpers ──────────────────────────────────────────────

        private void PreWarm()
        {
            if (_pool.Count > 0) return;

            for (int i = 0; i < initialSize; i++)
            {
                T instance = Instantiate(prefab, transform);
                instance.gameObject.SetActive(false);
                _pool.Add(instance);
            }
        }
    }
}
