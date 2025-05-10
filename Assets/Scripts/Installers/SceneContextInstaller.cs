using System;
using Core.EntryPoint;
using Game;
using Zenject;

namespace Installers
{
    public class SceneContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IMoveable>()
                .To<MoveComponent>()
                .AsCached();
            
            Container.Bind<IAnimation>()
                .To<AnimationController>()
                .AsCached();
            
            Container.Bind<IJumpable>()
                .To<JumpComponent>()
                .AsCached();
            
            Container.Bind<IRotatable>()
                .To<RotationComponent>()
                .AsCached();
            
            Container.Bind<ICamera>()
                .To<CameraComponent>()
                .AsCached();

            Container.Bind<EntryPoint>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<Player>()
                .FromComponentInHierarchy()
                .AsCached();

            Container
                .Bind<IPlayerCore>()
                .To<PlayerCore>()
                .AsCached()
                .NonLazy();
            
            Container.Bind(typeof(IPlayerInteractor), typeof(IDisposable))
                .To<PlayerInteractor>()
                .AsCached()
                .NonLazy();
            
            Container.Bind<Cam>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<Cube>()
                .FromComponentsInHierarchy()
                .AsCached();
        }
    }
}
