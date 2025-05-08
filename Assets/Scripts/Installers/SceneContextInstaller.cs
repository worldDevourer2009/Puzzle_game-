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

            Container.Bind<Player>()
                .FromComponentInHierarchy()
                .AsCached();
        }
    }
}
