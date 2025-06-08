using Game;
using Zenject;

namespace Installers
{
    public sealed class FirstLevelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ThrowableItem>()
                .FromComponentsInHierarchy()
                .AsCached();
        }
    }
}