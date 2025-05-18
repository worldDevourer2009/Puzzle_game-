using System;
using Game;
using Ui;
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

            Container.Bind<Cube>()
                .FromComponentsInHierarchy()
                .AsCached()
                .NonLazy();

            BindPlayers();
            BindMVP();
        }

        private void BindMVP()
        {
            Container.Bind<PauseResumeModel>()
                .AsSingle();

            Container.Bind<IPauseResumeView>()
                .FromComponentsInHierarchy()
                .AsCached()
                .NonLazy();

            Container.Bind<PauseResumeController>()
                .AsSingle()
                .NonLazy();
        }

        private void BindPlayers()
        {
            Container.Bind<ICameraController>()
                .To<PlayerCameraControllerComponent>()
                .AsCached();
            
            Container
                .Bind(typeof(IPlayerCore), typeof(IDisposable))
                .To<PlayerCore>()
                .AsSingle();
            
            Container.Bind(typeof(IPlayerInteractor), typeof(IDisposable))
                .To<PlayerInteractor>()
                .AsSingle();

            Container.Bind(typeof(IPlayerController), typeof(IDisposable))
                .To<PlayerController>()
                .AsSingle();
            
            Container.Bind(typeof(IAnimation), typeof(IDisposable))
                .To<AnimationController>()
                .AsCached();
        }
    }
}
