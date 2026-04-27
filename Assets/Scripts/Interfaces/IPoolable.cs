namespace Flexus.Interfaces
{
    /// <summary>
    /// Implemented by any object that participates in an object pool lifecycle.
    /// Consumers call <see cref="OnSpawn"/> after retrieving an object and
    /// <see cref="OnDespawn"/> before returning it to the pool.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>Called each time the object is taken from the pool.</summary>
        void OnSpawn();

        /// <summary>Called each time the object is returned to the pool.</summary>
        void OnDespawn();
    }
}
