using Ui;
using Zenject;

namespace Installers
{
    public class UIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<UIInitializer>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesTo<UISystem>()
                .AsSingle()
                .NonLazy();

            BindModels();
            BindPresenters();
        }

        private void BindModels()
        {
            Container.BindInterfacesAndSelfTo<PauseMenuModel>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<SettingsModel>()
                .AsSingle();
            
            Container.Bind<MainMenuModel>()
                .AsSingle();
            
            Container.Bind<HudModel>()
                .AsSingle();
        }

        private void BindPresenters()
        {
            Container.BindInterfacesAndSelfTo<PauseMenuPresenter>()
                .AsSingle()
                .NonLazy();
            
            Container.BindInterfacesAndSelfTo<HudPresenter>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<LoadingPresenter>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<MainMenuPresenter>()
                .AsSingle()
                .NonLazy();
            
            Container.BindInterfacesAndSelfTo<SettingsPresenter>()
                .AsSingle()
                .NonLazy();
        }
    }
}