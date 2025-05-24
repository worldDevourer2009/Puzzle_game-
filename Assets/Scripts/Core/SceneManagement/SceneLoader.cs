using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core
{
    public interface ISceneLoader
    {
        event Action OnLoad;
        event Action OnLoaded;
        event Action OnUnload;
        event Action OnUnloaded;
        UniTask LoadSceneById(string id, LoadMode loadSceneMode = LoadMode.Additive);
        UniTask UnloadSceneById(string id);
    }

    public enum LoadMode
    {
        Single,
        Additive
    }
    
    public class SceneLoader : ISceneLoader
    {
        public event Action OnLoad;
        public event Action OnLoaded;
        public event Action OnUnload;
        public event Action OnUnloaded;
        
        private const string LoadingSceneId = "LoadingScene";
        private readonly IAddressableLoader _addressableLoader;
        private readonly ILogger _logger;
        private readonly ICameraManager _cameraManager;
        private readonly Dictionary<string, SceneInstance> _sceneInstances;

        public SceneLoader(IAddressableLoader addressableLoader, ICameraManager cameraManager, ILogger logger)
        {
            _addressableLoader = addressableLoader;
            _cameraManager = cameraManager;
            _logger = logger;
            _sceneInstances = new Dictionary<string, SceneInstance>();
        }

        public async UniTask LoadSceneById(string id, LoadMode loadSceneMode = LoadMode.Additive)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Scene id is null");
                return;
            }
            
            OnLoad?.Invoke();

            _cameraManager.DestroyAllCameras();
            var loadingScene = await _addressableLoader.LoadScene(LoadingSceneId, LoadSceneMode.Single);
            await _cameraManager.SetActiveCamera(CustomCameraType.LoadCamera);
            SceneManager.SetActiveScene(loadingScene.Scene);

            LoadSceneMode sceneMode;

            switch (loadSceneMode)
            {
                case LoadMode.Single:
                    sceneMode = LoadSceneMode.Single;
                    break;
                case LoadMode.Additive:
                    sceneMode = LoadSceneMode.Additive;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(loadSceneMode), loadSceneMode, null);
            }
            
            var targetScene = await _addressableLoader.LoadScene(id, sceneMode);
            SceneManager.SetActiveScene(targetScene.Scene);
            _sceneInstances[id] = targetScene;
            OnLoaded?.Invoke();
            
            await _addressableLoader.UnloadScene(loadingScene);
        }

        private bool IsSceneLoaded(string id)
        {
            return _sceneInstances.TryGetValue(id, out var scene);
        }

        public async UniTask UnloadSceneById(string id)
        {
            if (_sceneInstances.TryGetValue(id, out var scene))
            {
                OnUnload?.Invoke();
                await _addressableLoader.UnloadScene(scene);
                OnUnloaded?.Invoke();
                _sceneInstances.Remove(id);
            }
            else
            {
                _logger.LogWarning($"Can't unload scene with id {id}");
            }
        }
    }
}