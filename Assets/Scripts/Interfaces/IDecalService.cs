using UnityEngine;

namespace Flexus.Interfaces
{
    /// <summary>
    /// Paints a decal onto the surface at a raycast hit point.
    /// Decouples projectile impact logic from texture-painting details.
    /// </summary>
    public interface IDecalService
    {
        /// <summary>
        /// Applies a decal to the surface described by <paramref name="hit"/>.
        /// </summary>
        void PaintDecal(RaycastHit hit);
    }
}
