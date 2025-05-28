using Core;
using Zenject;

namespace Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<EntryPoint>()
                .FromComponentsInHierarchy()
                .AsSingle()
                .NonLazy();
        }
    }
}