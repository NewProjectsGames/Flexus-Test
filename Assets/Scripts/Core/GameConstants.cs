using UnityEngine;

namespace Flexus.Core
{
    /// <summary>
    /// Project-wide physical constants used in ballistic calculations.
    /// Centralised here so any future tuning change propagates automatically.
    /// </summary>
    public static class GameConstants
    {
        /// <summary>Gravitational acceleration (m/s²) applied to projectiles.</summary>
        public const float Gravity = 9.81f;

        /// <summary>
        /// Energy-retention coefficient applied after a surface bounce.
        /// Range [0, 1]: 0 = fully inelastic, 1 = perfectly elastic.
        /// </summary>
        public const float Bounce = 0.5f;

        /// <summary>Tag assigned to destructible wall surfaces.</summary>
        public const string WallTag = "Wall";

        /// <summary>
        /// Squared speed threshold below which a projectile is considered stationary
        /// and should explode rather than continue bouncing.
        /// </summary>
        public const float MinVelocitySqr = 0.2f;

        /// <summary>
        /// Calculates reflected velocity, applying extra damping for head-on collisions.
        /// </summary>
        public static Vector3 CalculateBounceVelocity(Vector3 currentVelocity, Vector3 normal)
        {
            Vector3 reflected = Vector3.Reflect(currentVelocity, normal);
            // Use dot product to check for head-on collision (incidence angle)
            float dot = Vector3.Dot(currentVelocity.normalized, normal);
            
            // If dot is close to -1, it's a head on collision.
            return reflected * (dot <= -0.5f ? Bounce / 10f : Bounce);
        }
    }
}
