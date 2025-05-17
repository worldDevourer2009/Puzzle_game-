using Core;
using Zenject;

namespace Installers
{
    public class ProjectContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IContextResolver>()
                .To<ContextResolver>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<IJsonSerializer>()
                .To<JsonSerializer>()
                .AsSingle()
                .NonLazy();

            Container.Bind<ISaveSystem>()
                .To<SaveSystem>()
                .AsSingle()
                .NonLazy();
            
            BindConfigs();

            Container.Bind<IAsyncGroupLoader>()
                .To<AsyncGroupLoader>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<IAddressableLoader>()
                .To<AddressablesCustomLoader>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind<IRaycaster>()
                .To<RaycasterSystem>()
                .AsSingle();
            
            Container
                .Bind<ICameraManager>()
                .To<CameraManager>()
                .AsSingle();

            Container.Bind<ILogger>()
                .To<Logger>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<IPoolSystem>()
                .To<PoolSystem>()
                .AsSingle()
                .NonLazy();

            Container.Bind<IFactorySystem>()
                .To<FactorySystem>()
                .AsSingle()
                .NonLazy();
            
            Container.BindInterfacesTo<GameLoop>()
                .AsSingle()
                .NonLazy();

            Container.Bind<IInput>()
                .To<InputSystem>()
                .AsSingle()
                .NonLazy();

            Container.Bind<IInputSource>()
                .To<KeyboardSource>()
                .AsCached()
                .NonLazy();
            
            Container.Bind<ILookSource>()
                .To<LookSourceMouse>()
                .AsCached()
                .NonLazy();
            
            Container.Bind<IInteractorCore>()
                .To<InteractorCore>()
                .AsSingle();

            Container.Bind<ILevelManager>()
                .To<LevelManagerCore>()
                .AsSingle()
                .NonLazy();
        }

        private void BindConfigs()
        {
            Container.Bind<InputConfig>()
                .FromResource("Configs/InputConfig")
                .AsSingle();

            Container.Bind<PlayerDefaultStatsConfig>()
                .FromResource("Configs/PlayerDefaultStatsConfig")
                .AsSingle();
            
            Container.Bind<PlayerInteractionConfig>()
                .FromResource("Configs/PlayerInteractionConfig")
                .AsSingle();
            
            Container.Bind<LevelsConfig>()
                .FromResource("Configs/LevelsConfig")
                .AsSingle();
            
            Container.Bind<AddressablesIdsConfig>()
                .FromResource("Configs/AddressablesIds")
                .AsSingle();
        }
    }
}