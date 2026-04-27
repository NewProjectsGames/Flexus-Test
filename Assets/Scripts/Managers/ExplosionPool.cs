using Flexus.Pool;

namespace Flexus.Managers
{
    /// <summary>
    /// Concrete, non-generic subclass of <see cref="ObjectPool{T}"/> for explosion FX prefabs.
    ///
    /// <para>
    /// Unity's Inspector cannot serialize open generic MonoBehaviour fields, so every
    /// usage of <see cref="ObjectPool{T}"/> that must appear in the Inspector requires
    /// a concrete sealed subclass like this one.
    /// Add this component to a GameObject alongside the FX prefab reference,
    /// then assign it to <see cref="FxManager"/>'s <c>Explosion Pool</c> field.
    /// </para>
    /// </summary>
    public sealed class ExplosionPool : ObjectPool<PoolableObject> { }
}
