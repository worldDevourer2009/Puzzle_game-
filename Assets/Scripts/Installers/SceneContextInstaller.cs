using Core;
using Zenject;

namespace Installers
{
    public class SceneContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IInputSystemController>()
                .To<InputSystemController>()
                .AsSingle()
                .NonLazy();
        }
    }
}
