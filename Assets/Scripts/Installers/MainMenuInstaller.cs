using Ui;
using Zenject;

namespace Installers
{
    public class MainMenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindModels();
            BindPresenters();
            BindViews();
        }

        private void BindModels()
        {
            Container.Bind<StartNewGameModel>()
                .AsSingle();
        }

        private void BindPresenters()
        {
            Container.Bind<StartNewGamePresenter>()
                .AsSingle();
        }

        private void BindViews()
        {
            Container.Bind<IStartNewGameView>()
                .To<StartNewGameView>()
                .FromComponentsInHierarchy()
                .AsCached();
        }
    }
}