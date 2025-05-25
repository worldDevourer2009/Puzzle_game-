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
                .To<MoveLogic>()
                .AsCached();
            
            Container.Bind<IStepable>()
                .To<StepLogic>()
                .AsCached();
            
            Container.Bind<IJumpable>()
                .To<JumpLogic>()
                .AsCached();
            
            Container.Bind<IGroundable>()
                .To<GroundableLogic>()
                .AsCached();
            
            Container.Bind<IRotatable>()
                .To<RotationLogic>()
                .AsCached();

            Container.Bind<Cube>()
                .FromComponentsInHierarchy()
                .AsCached();

            BindPlayers();
            BindMVP();
        }

        private void BindMVP()
        {
            Container.BindInterfacesAndSelfTo<PauseResumeModel>()
                .AsSingle();

            Container.Bind<IPauseResumeView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<PauseMenuPresenter>()
                .AsSingle()
                .NonLazy();
        }

        private void BindPlayers()
        {
            Container.Bind(typeof(IPlayerInputHandler), typeof(IInitializable), typeof(IDisposable))
                .To<PlayerInputHandler>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<IPlayerCameraLogic>()
                .To<PlayerCameraLogic>()
                .AsSingle();
            
            Container
                .Bind(typeof(IPlayerCore), typeof(IDisposable))
                .To<PlayerCore>()
                .AsSingle();
            
            Container.BindInterfacesTo<PlayerInteractor>()
                .AsSingle();

            Container.Bind(typeof(IPlayerController), typeof(IDisposable))
                .To<PlayerController>()
                .AsSingle();
            
            Container.Bind(typeof(IAnimation), typeof(IDisposable))
                .To<PlayerAnimationController>()
                .AsCached();
        }
    }
}
