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
        }

        private void BindPresenters()
        {
        }

        private void BindViews()
        {
            // Container.Bind<IStartNewGameView>()
            //     .To<StartNewGameView>()
            //     .FromComponentsInHierarchy()
            //     .AsCached();
        }
    }
}