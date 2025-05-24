using ZLinq;
using Cysharp.Threading.Tasks;
using Game;
using ModestTree;

namespace Core
{
    public interface IGameManager
    {
        UniTask LoadMainMenu();
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
        private readonly ScenesConfig _scenesConfig;
        private readonly IPlayerDataHolder _playerDataHolder;
        private readonly ISceneLoader _sceneLoader;
        private readonly ILevelManager _levelManager;
        private readonly ICameraManager _cameraManager;
        private readonly LevelsConfig _levelsConfig;
        private readonly IGameLoop _gameLoop;
        private readonly ILogger _logger;

        private string _lastLoadedLevel;

        public GameManager(ISceneLoader sceneLoader, ILevelManager levelManager, ScenesConfig scenesConfig, 
            ICameraManager cameraManager, LevelsConfig levelsConfig, IGameLoop gameLoop, IPlayerDataHolder playerDataHolder,  ILogger logger)
        {
            _sceneLoader = sceneLoader;
            _levelManager = levelManager;
            _scenesConfig = scenesConfig;
            _cameraManager = cameraManager;
            _levelsConfig = levelsConfig;
            _gameLoop = gameLoop;
            _playerDataHolder = playerDataHolder;
            _logger = logger;
        }

        public async UniTask LoadMainMenu()
        {
            var firstScene = _scenesConfig.Scenes.AsValueEnumerable().FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

            if (firstScene == null || firstScene.IsEmpty())
            {
                _logger.LogWarning("Can't find first scene");
                return;
            }
            
            var op = _sceneLoader.LoadSceneById(firstScene, LoadMode.Single);

            await op;
            
            await _cameraManager.CreateCamera(CustomCameraType.UiCamera);
        }

        public async UniTask LaunchGame()
        {
            var firstScene = _scenesConfig.Scenes.AsValueEnumerable().FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

            if (firstScene == null || firstScene.IsEmpty())
            {
                _logger.LogWarning("Can't find first scene");
                return;
            }
            
            var op = _sceneLoader.LoadSceneById(firstScene, LoadMode.Additive);

            await op;
            
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
            
            await _playerDataHolder.InitData();
            var op = _levelManager.LoadLevelByName(levelName.LevelName);
            await op;
        }

        public async UniTask RestartLevel()
        {
            var op= _levelManager.LoadCurrentLevel();
            await op;
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