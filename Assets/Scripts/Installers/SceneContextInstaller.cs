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

            Container.Bind<Player>()
                .FromComponentInHierarchy()
                .AsCached();
            
            Container.Bind<Cam>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}
