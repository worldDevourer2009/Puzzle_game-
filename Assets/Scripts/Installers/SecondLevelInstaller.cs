using Game;
using Zenject;

namespace Installers
{
    public sealed class SecondLevelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<Door>()
                .FromComponentInHierarchy()
                .AsCached();
        }
    }
}