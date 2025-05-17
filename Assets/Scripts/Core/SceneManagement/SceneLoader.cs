using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core
{
    public interface ISceneLoader
    {
        UniTask LoadSceneById(string id, LoadSceneMode loadSceneMode = LoadSceneMode.Additive);
        UniTask UnloadSceneById(string id);
    }
    
    public class SceneLoader : ISceneLoader
    {
        private readonly IAddressableLoader _addressableLoader;
        private readonly ILogger _logger;
        private readonly Dictionary<string, SceneInstance> _sceneInstances;

        public SceneLoader(IAddressableLoader addressableLoader, ILogger logger)
        {
            _addressableLoader = addressableLoader;
            _logger = logger;
            _sceneInstances = new Dictionary<string, SceneInstance>();
        }

        public async UniTask LoadSceneById(string id, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            var scene = await _addressableLoader.LoadScene(id, loadSceneMode);
            _sceneInstances.TryAdd(id, scene);
        }

        public async UniTask UnloadSceneById(string id)
        {
            if (_sceneInstances.TryGetValue(id, out var scene))
            {
                await _addressableLoader.UnloadScene(scene);
            }
            else
            {
                _logger.LogWarning($"Can't unload scene with id {id}");
            }
        }
    }
}