using System;
using Core;
using Zenject;

namespace Installers
{
    public class ProjectContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameLoopRunner>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<IGameManager>()
                .To<GameManager>()
                .AsSingle()
                .NonLazy();
            
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

            Container.Bind<ISaver>()
                .To<Saver>()
                .AsSingle()
                .NonLazy();

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

            Container.BindInterfacesTo<InputSystem>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesTo<InputSourceComposite>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<IInputSource>()
                .To<KeyboardSource>()
                .AsCached()
                .NonLazy();

            Container.BindInterfacesTo<AudioSystem>()
                .AsSingle()
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
            
            Container.Bind<ISceneLoader>()
                .To<SceneLoader>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesTo<CursorManager>()
                .AsSingle()
                .NonLazy();
            
            Container.BindInterfacesTo<AudioManager>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind(typeof(IPlayerDataHolder), typeof(IDisposable))
                .To<PlayerDataHolder>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<ExposedSystemDataHolder>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesTo<InternalSystemDataHolder>()
                .AsSingle()
                .NonLazy();

            BindStates();
        }

        private void BindStates()
        {
            Container.Bind<IState>()
                .To<MainMenuState>()
                .AsTransient()
                .NonLazy();
            
            Container.Bind<IState>()
                .To<NewGameState>()
                .AsTransient()
                .NonLazy();
            
            Container.Bind<IState>()
                .To<PauseState>()
                .AsTransient()
                .NonLazy();
            
            Container.Bind<IState>()
                .To<ResumeState>()
                .AsTransient()
                .NonLazy();

            Container.Bind<IGameStateFactory>()
                .To<GameStateFactory>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<IGameStateManager>()
                .To<GameStateManager>()
                .AsSingle()
                .NonLazy();
        }
    }
}