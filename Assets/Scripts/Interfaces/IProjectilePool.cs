using UnityEngine;

namespace Flexus.Interfaces
{
    /// <summary>
    /// Provides pre-built projectile instances for firing.
    /// Decouples <see cref="Flexus.Cannon.CannonController"/> from the
    /// concrete pooling / mesh-generation implementation.
    /// </summary>
    public interface IProjectilePool
    {
        /// <summary>Returns the next available projectile GameObject.</summary>
        GameObject GetProjectile();
    }
}
