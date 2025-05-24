using System;
using Game;
using Zenject;

namespace Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ICameraController>()
                .To<PlayerCameraLogic>()
                .AsCached()
                .NonLazy();

            Container.Bind(typeof(IPlayerInteractor), typeof(IDisposable))
                .To<PlayerInteractor>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind(typeof(IPlayerController), typeof(IDisposable))
                .To<PlayerController>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind(typeof(IPlayerCore), typeof(IDisposable))
                .To<PlayerCore>()
                .AsSingle();

            Container.Bind(typeof(IAnimation), typeof(IDisposable))
                .To<PlayerAnimationController>()
                .AsCached();
        }
    }
}