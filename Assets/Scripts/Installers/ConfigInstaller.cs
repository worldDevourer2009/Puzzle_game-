using Core;
using Zenject;

namespace Installers
{
    public class ConfigInstaller : MonoInstaller
    {
        public override void InstallBindings()
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
            
            Container.Bind<ScenesConfig>()
                .FromResource("Configs/ScenesConfig")
                .AsSingle();
            
            Container.Bind<CursorConfig>()
                .FromResource("Configs/CursorConfig")
                .AsSingle();
            
            Container.Bind<InternalSettingsConfig>()
                .FromResource("Configs/InternalSettingsConfig")
                .AsSingle();
            
            Container.Bind<AudioDataConfig>()
                .FromResource("Configs/AudioDataConfig")
                .AsSingle();
            
            Container.Bind<ExternalSettingsConfig>()
                .FromResource("Configs/ExternalSettingsConfig")
                .AsSingle();
        }
    }
}