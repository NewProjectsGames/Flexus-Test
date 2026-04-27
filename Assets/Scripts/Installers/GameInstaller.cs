using Flexus.Camera;
using Flexus.Interfaces;
using Flexus.Managers;
using Flexus.Visual;
using UnityEngine;
using Zenject;

namespace Flexus.Installers
{
    /// <summary>
    /// Zenject <see cref="MonoInstaller"/> for the main game scene.
    /// Attach this to a <c>SceneContext</c> GameObject in the scene hierarchy.
    ///
    /// <para>
    /// Every service binding in this file answers the question:
    /// <i>"When something asks for interface X, give it concrete Y."</i>
    /// Callers never import concrete types — only interfaces from <c>Flexus.Interfaces</c>.
    /// </para>
    ///
    /// <para><b>Binding strategy:</b></para>
    /// <list type="bullet">
    ///   <item><description><c>AsSingle()</c> — one instance shared across the entire scene.</description></item>
    ///   <item><description><c>FromComponentInHierarchy()</c> — resolves a MonoBehaviour already placed in the scene.</description></item>
    /// </list>
    /// </summary>
    public sealed class GameInstaller : MonoInstaller
    {
        // ── Scene references ─────────────────────────────────────────────
        // Assign these in the Inspector on the SceneContext GameObject.

        [Header("Services")]
        [Tooltip("CameraShake MonoBehaviour already present in the scene.")]
        [SerializeField] private CameraShake cameraShake;

        [Tooltip("FxManager MonoBehaviour already present in the scene.")]
        [SerializeField] private FxManager fxManager;

        [Tooltip("DecalPainter MonoBehaviour already present in the scene.")]
        [SerializeField] private DecalPainter decalPainter;

        [Tooltip("TrajectoryController MonoBehaviour already present in the scene.")]
        [SerializeField] private TrajectoryController trajectoryController;

        [Tooltip("ProjectileManager MonoBehaviour already present in the scene.")]
        [SerializeField] private ProjectileManager projectileManager;

        // ── MonoInstaller ────────────────────────────────────────────────

        /// <summary>
        /// Called by Zenject during scene initialisation to register all bindings.
        /// </summary>
        public override void InstallBindings()
        {
            BindCameraShake();
            BindFxService();
            BindDecalService();
            BindTrajectoryService();
            BindProjectilePool();
        }

        // ── Private binding helpers ──────────────────────────────────────

        /// <summary>Binds the camera shake service to its concrete MonoBehaviour.</summary>
        private void BindCameraShake()
        {
            Debug.Assert(cameraShake != null, $"[{nameof(GameInstaller)}] CameraShake reference is missing!", this);
            Container.Bind<ICameraShakeService>().FromInstance(cameraShake).AsSingle().NonLazy();
        }

        /// <summary>Binds the explosion FX service.</summary>
        private void BindFxService()
        {
            Debug.Assert(fxManager != null, $"[{nameof(GameInstaller)}] FxManager reference is missing!", this);
            Container.Bind<IFxService>().FromInstance(fxManager).AsSingle().NonLazy();
        }

        /// <summary>Binds the decal painting service.</summary>
        private void BindDecalService()
        {
            Debug.Assert(decalPainter != null, $"[{nameof(GameInstaller)}] DecalPainter reference is missing!", this);
            Container.Bind<IDecalService>().FromInstance(decalPainter).AsSingle().NonLazy();
        }

        /// <summary>Binds the trajectory preview service.</summary>
        private void BindTrajectoryService()
        {
            Debug.Assert(trajectoryController != null, $"[{nameof(GameInstaller)}] TrajectoryController reference is missing!", this);
            Container.Bind<ITrajectoryService>().FromInstance(trajectoryController).AsSingle().NonLazy();
        }

        /// <summary>Binds the projectile pool service.</summary>
        private void BindProjectilePool()
        {
            Debug.Assert(projectileManager != null, $"[{nameof(GameInstaller)}] ProjectileManager reference is missing!", this);
            Container.Bind<IProjectilePool>().FromInstance(projectileManager).AsSingle().NonLazy();
        }
    }
}
