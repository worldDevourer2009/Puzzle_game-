using System;
using Game;
using Zenject;

namespace Installers
{
    public sealed class SceneContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IMoveable>()
                .To<MoveComponent>()
                .AsCached();
            
            Container.Bind(typeof(IAnimation), typeof(IDisposable))
                .To<AnimationController>()
                .AsCached();
            
            Container.Bind<IJumpable>()
                .To<JumpComponent>()
                .AsCached()
                .NonLazy();
            
            Container.Bind<IGroundable>()
                .To<GroundableComponent>()
                .AsCached()
                .NonLazy();
            
            Container.Bind<IRotatable>()
                .To<RotationComponent>()
                .AsCached()
                .NonLazy();

            Container.Bind<ICameraController>()
                .To<PlayerCameraControllerComponent>()
                .AsCached()
                .NonLazy();
            
            Container
                .Bind(typeof(IPlayerCore), typeof(IDisposable))
                .To<PlayerCore>()
                .AsSingle();
            
            Container.Bind(typeof(IPlayerInteractor), typeof(IDisposable))
                .To<PlayerInteractor>()
                .AsSingle()
                .NonLazy();

            Container.Bind(typeof(IPlayerController), typeof(IDisposable))
                .To<PlayerController>()
                .AsSingle()
                .NonLazy();

            Container.Bind<Cube>()
                .FromComponentsInHierarchy()
                .AsCached()
                .NonLazy();
        }
    }
}
