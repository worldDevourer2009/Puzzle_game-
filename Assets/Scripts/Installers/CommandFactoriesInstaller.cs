using Core;
using Zenject;

namespace Installers
{
    public class CommandFactoriesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ILevelCommandsFactory>()
                .To<LevelCommandsFactory>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<ISceneCommandsFactory>()
                .To<SceneCommandsFactory>()
                .AsSingle()
                .NonLazy();
        }
    }
}