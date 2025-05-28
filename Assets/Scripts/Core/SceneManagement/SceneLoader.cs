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
        private const string BootstrapSceneId = "Bootstrap";
        
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
            
            var loadingScene = await _addressableLoader.LoadScene(LoadingSceneId);
            SceneManager.SetActiveScene(loadingScene.Scene);
            await _cameraManager.SetActiveCamera(CustomCameraType.LoadCamera);

            foreach (var kvp in _sceneInstances)
            {
                if (kvp.Key != BootstrapSceneId && kvp.Key != LoadingSceneId)
                {
                    await _addressableLoader.UnloadScene(kvp.Value);
                }
            }
            
            var keysToKeep = new[] { BootstrapSceneId, LoadingSceneId };
            foreach (var key in new List<string>(_sceneInstances.Keys))
            {
                if (Array.IndexOf(keysToKeep, key) == -1)
                {
                    _sceneInstances.Remove(key);
                }
            }
            
            var targetScene = await _addressableLoader.LoadScene(id);
            SceneManager.SetActiveScene(targetScene.Scene);
            _sceneInstances[id] = targetScene;
            
            OnLoaded?.Invoke();
            
            await _addressableLoader.UnloadScene(loadingScene);
            _sceneInstances.Remove(LoadingSceneId);
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