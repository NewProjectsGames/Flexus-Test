using UnityEngine;

namespace Flexus.Interfaces
{
    /// <summary>
    /// Provides visual effect playback. Implementations may use particle pools,
    /// addressable assets, or any other approach without affecting callers.
    /// </summary>
    public interface IFxService
    {
        /// <summary>
        /// Spawns an explosion effect at the given world position,
        /// oriented along <paramref name="normal"/>.
        /// </summary>
        void PlayExplosion(Vector3 position, Vector3 normal);
    }
}
