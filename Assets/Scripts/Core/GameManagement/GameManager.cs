using System.Threading.Tasks;
using ZLinq;
using Cysharp.Threading.Tasks;
using ModestTree;

namespace Core
{
    public interface IGameManager
    {
        UniTask LaunchGame();
        UniTask StartNewGame();
        UniTask LoadLevel();
        UniTask RestartLevel();
        UniTask PauseGame();
        UniTask UnpauseGame();
        UniTask SaveGame();
        UniTask QuitGame();
    }
    
    public class GameManager : IGameManager
    {
        private readonly ILevelCommandsFactory _levelCommandsFactory;
        private readonly ISceneCommandsFactory _sceneCommandsFactory;
        private readonly ScenesConfig _scenesConfig;
        private readonly ICameraManager _cameraManager;
        private readonly LevelsConfig _levelsConfig;
        private readonly IGameLoop _gameLoop;
        private readonly ILogger _logger;

        private string _lastLoadedLevel;

        public GameManager(ILevelCommandsFactory levelCommandsFactory, 
            ISceneCommandsFactory sceneCommandsFactory, ScenesConfig scenesConfig, 
            ICameraManager cameraManager, LevelsConfig levelsConfig, IGameLoop gameLoop, ILogger logger)
        {
            _levelCommandsFactory = levelCommandsFactory;
            _sceneCommandsFactory = sceneCommandsFactory;
            _scenesConfig = scenesConfig;
            _cameraManager = cameraManager;
            _levelsConfig = levelsConfig;
            _gameLoop = gameLoop;
            _logger = logger;
        }

        public async UniTask LaunchGame()
        {
            var firstScene = _scenesConfig.Scenes.AsValueEnumerable().FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

            if (firstScene == null || firstScene.IsEmpty())
            {
                return;
            }
            
            var command = await _sceneCommandsFactory.CreateCommand(firstScene, LoadMode.Single);

            if (command != null)
            {
                _cameraManager.UnloadCameras();
                await command.ExecuteAsync();
            }
            
            await _cameraManager.CreateCamera(CustomCameraType.UiCamera);
        }

        public async UniTask StartNewGame()
        {
            var firstLevelName = _levelsConfig.LevelData.AsValueEnumerable().FirstOrDefault();
            await LoadLevelHelper(firstLevelName);
        }

        public async UniTask LoadLevel()
        {
            var levelName = _levelsConfig.LastLevelData;

            if (string.IsNullOrEmpty(levelName.LevelName))
            {
                await StartNewGame();
                return;
            }
            
            await LoadLevelHelper(levelName);
        }

        private async UniTask LoadLevelHelper(LevelData levelName)
        {
            if (string.IsNullOrWhiteSpace(levelName.LevelName))
            {
                _logger.LogWarning("Can't get level name");
                return;
            }
            
            var command = await _levelCommandsFactory.CreateCommand(levelName.LevelName);

            if (command != null)
            {
                await command.ExecuteAsync();
                _lastLoadedLevel = levelName.LevelName;
            }
            else
            {
                _logger.LogWarning($"Can't create command for level with name {levelName.LevelName}");
            }
        }

        public async UniTask RestartLevel()
        {
            var command = await _levelCommandsFactory.CreateCommand(_lastLoadedLevel);
            
            if (command != null)
            {
                await command.ExecuteAsync();
            }
        }

        public UniTask PauseGame()
        {
            _gameLoop.EnableUpdate(false);
            return UniTask.CompletedTask;
        }

        public UniTask UnpauseGame()
        {
            _gameLoop.EnableUpdate(true);
            return UniTask.CompletedTask;
        }

        public async UniTask SaveGame()
        {
            //throw new System.NotImplementedException();
        }

        public async UniTask QuitGame()
        {
            //throw new System.NotImplementedException();
        }
    }
}