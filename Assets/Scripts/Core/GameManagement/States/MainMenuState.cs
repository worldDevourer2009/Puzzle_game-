using Cysharp.Threading.Tasks;
using UnityEngine;
using ZLinq;

namespace Core
{
    public class MainMenuState : IState
    {
        public GameState Name => GameState.MainMenu;

        private readonly ISceneLoader _sceneLoader;
        private readonly IGameLoop _gameLoop;
        private readonly ICameraManager _cameraManager;
        private readonly ILogger _logger;
        
        private readonly ScenesConfig _scenesConfig;

        public MainMenuState(ISceneLoader sceneLoader, IGameLoop gameLoop, ICameraManager cameraManager, ILogger logger, ScenesConfig scenesConfig)
        {
            _sceneLoader = sceneLoader;
            _gameLoop = gameLoop;
            _cameraManager = cameraManager;
            _logger = logger;
            _scenesConfig = scenesConfig;
        }

        public async UniTask OnEnter()
        {
            var mainMenu = _scenesConfig.Scenes.AsValueEnumerable().FirstOrDefault();
            
            if (string.IsNullOrWhiteSpace(mainMenu))
            {
                _logger.Log("Can't find main menu scene");
                return;
            }
            
            await _sceneLoader.LoadSceneById(mainMenu);
            await _cameraManager.CreateCamera(CustomCameraType.UiCamera);
            _gameLoop.EnableUpdate(false);
        }

        public UniTask OnUpdate()
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnExit()
        {
            return UniTask.CompletedTask;
        }

        public GameState? NextState(GameTrigger trigger)
        {
            return trigger == GameTrigger.StartGame 
                ? GameState.NewGame 
                : null;
        }
    }
}