using Ui;
using Zenject;

namespace Installers
{
    public class LoadingContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<LoadingModel>()
                .AsSingle();

            Container.Bind<ILoadingView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<LoadingPresenter>()
                .AsSingle()
                .NonLazy();
        }
    }
}