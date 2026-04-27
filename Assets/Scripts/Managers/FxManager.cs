using Flexus.Interfaces;
using Flexus.Pool;
using UnityEngine;

namespace Flexus.Managers
{
    /// <summary>
    /// Concrete implementation of <see cref="IFxService"/> that retrieves explosion
    /// particles from an <see cref="ObjectPool{T}"/> and places them at impact points.
    ///
    /// <para>
    /// Bound as a single instance via Zenject; zero static state, fully injectable.
    /// </para>
    /// </summary>
    public sealed class FxManager : MonoBehaviour, IFxService
    {
        // ── Inspector ────────────────────────────────────────────────────

        [Tooltip("Pool that supplies explosion particle prefab instances. Assign an ExplosionPool component here.")]
        [SerializeField] private ExplosionPool explosionPool;

        // ── IFxService ───────────────────────────────────────────────────

        /// <inheritdoc/>
        public void PlayExplosion(Vector3 position, Vector3 normal)
        {
            PoolableObject fx = explosionPool.Get();

            // Brief deactivate-reactivate cycle ensures particle systems restart cleanly.
            fx.gameObject.SetActive(false);
            fx.transform.position = position;
            fx.transform.rotation = Quaternion.LookRotation(normal);
            fx.gameObject.SetActive(true);
        }
    }
}
