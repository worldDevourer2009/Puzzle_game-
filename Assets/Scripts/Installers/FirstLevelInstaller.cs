using Game;
using Zenject;

namespace Installers
{
    public sealed class FirstLevelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<NextLevelObject>()
                .FromComponentsInHierarchy()
                .AsCached();
        }
    }
}