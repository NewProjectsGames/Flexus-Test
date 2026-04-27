namespace Flexus.Interfaces
{
    /// <summary>
    /// Triggers a camera shake impulse.
    /// Callers do not need to know which camera is shaking or how.
    /// </summary>
    public interface ICameraShakeService
    {
        /// <summary>Triggers a shake of preconfigured duration and intensity.</summary>
        void ShakeCamera();
    }
}
