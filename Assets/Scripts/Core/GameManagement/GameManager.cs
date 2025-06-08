#if UNITY_EDITOR
using UnityEditor;
#endif
using ZLinq;
using Cysharp.Threading.Tasks;
using ModestTree;
using UnityEngine;

namespace Core
{
    public interface IGameManager
    {
        UniTask LoadMainMenu();
        UniTask LaunchGame();
        UniTask StartNewGame();
        UniTask LoadGame(string saveName);
        UniTask LoadLevel();
        UniTask RestartLevel();
        UniTask PauseGame();
        UniTask UnpauseGame();
        UniTask QuitGame();
    }

    public class GameManager : IGameManager
    {
        private readonly ScenesConfig _scenesConfig;
        private readonly ISceneLoader _sceneLoader;
        private readonly ILevelManager _levelManager;
        private readonly ICameraManager _cameraManager;
        private readonly ISaver _saver;
        private readonly ILoader _loader;
        private readonly LevelsConfig _levelsConfig;
        private readonly IGameLoop _gameLoop;

        private string _lastLoadedLevel;
        
        public GameManager(ScenesConfig scenesConfig, ISceneLoader sceneLoader,
            ILevelManager levelManager, ICameraManager cameraManager, ISaver saver, LevelsConfig levelsConfig,
            IGameLoop gameLoop, ILoader loader)
        {
            _scenesConfig = scenesConfig;
            _sceneLoader = sceneLoader;
            _levelManager = levelManager;
            _cameraManager = cameraManager;
            _saver = saver;
            _levelsConfig = levelsConfig;
            _gameLoop = gameLoop;
            _loader = loader;
        }

        public async UniTask LoadMainMenu()
        {
            var firstScene = _scenesConfig.Scenes.AsValueEnumerable()
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

            if (firstScene == null || firstScene.IsEmpty())
            {
                Logger.Instance.LogWarning("Can't find first scene");
                return;
            }

            var op = _sceneLoader.LoadSceneById(firstScene, LoadMode.Single);

            await op;

            await _cameraManager.CreateCamera(CustomCameraType.UiCamera);
        }

        public async UniTask LaunchGame()
        {
            var firstScene = _scenesConfig.Scenes.AsValueEnumerable()
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

            if (firstScene == null || firstScene.IsEmpty())
            {
                Logger.Instance.LogWarning("Can't find first scene");
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

        //TODO make load by save name
        public async UniTask LoadGame(string saveName)
        {
            var firstSave = _loader.ListSaves().AsValueEnumerable().FirstOrDefault();
            
            var data = await _loader.Load(firstSave);
            Logger.Instance.Log($"Loaded data level {data.LevelIndex}");
            await _levelManager.LoadLevelByIndex(data.LevelIndex);
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
                Logger.Instance.LogWarning("Can't get level name");
                return;
            }
            
            var op = _levelManager.LoadLevelByName(levelName.LevelName);
            await op;
        }

        public async UniTask RestartLevel()
        {
            var op = _levelManager.LoadCurrentLevel();
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

        public async UniTask QuitGame()
        {
            await _saver.SaveAll();
            
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}