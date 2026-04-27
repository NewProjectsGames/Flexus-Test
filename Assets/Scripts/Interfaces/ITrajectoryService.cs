using UnityEngine;

namespace Flexus.Interfaces
{
    /// <summary>
    /// Computes and renders a predicted projectile trajectory.
    /// The concrete implementation controls rendering (LineRenderer, dots, etc.).
    /// </summary>
    public interface ITrajectoryService
    {
        /// <summary>
        /// Recalculates and redraws the trajectory preview.
        /// </summary>
        /// <param name="startPoint">Launch transform (position + forward direction).</param>
        /// <param name="power">Launch power scalar.</param>
        void CalculateTrajectory(Transform startPoint, float power);
    }
}
