using Core;
using Zenject;

namespace Installers
{
    public class ProjectContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IJsonSerializer>()
                .To<JsonSerializer>()
                .AsSingle()
                .NonLazy();

            Container.Bind<ISaveSystem>()
                .To<SaveSystem>()
                .AsSingle()
                .NonLazy();
            
            BindConfigs();

            Container.Bind<ILogger>()
                .To<Logger>()
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
        }

        private void BindConfigs()
        {
            Container.Bind<InputConfig>()
                .FromResource("Configs/InputConfig")
                .AsSingle();

            Container.Bind<PlayerDefaultStatsConfig>()
                .FromResource("Configs/PlayerDefaultStatsConfig")
                .AsSingle();
        }
    }
}