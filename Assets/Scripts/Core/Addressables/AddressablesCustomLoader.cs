using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core
{
    public interface IAddressableLoader
    {
        event Action<float> OnProgress;
        UniTask<T> LoadResource<T>(string id);
        UniTask<GameObject> Instantiate(string id);
        UniTask<T> Instantiate<T>(string id) where T : Component;
        UniTask<SceneInstance> LoadScene(string id, LoadSceneMode loadSceneMode = LoadSceneMode.Additive);
        void UnloadResource<T>(T asset);
        UniTask UnloadScene(SceneInstance scene);
    }

    public class AddressablesCustomLoader : IAddressableLoader
    {
        public event Action<float> OnProgress;
        private readonly ILogger _logger;

        public AddressablesCustomLoader(ILogger logger)
        {
            _logger = logger;
        }

        public async UniTask<T> LoadResource<T>(string id)
        {
            var asyncOp = Addressables.LoadAssetAsync<T>(id);
            try
            {
                var result = await asyncOp.ToUniTask();
                return result;
            }
            catch
            {
                _logger.LogWarning($"Can't load asset at path {id}");
                return default;
            }
        }

        public async UniTask<GameObject> Instantiate(string id)
        {
            var asyncOp = Addressables.InstantiateAsync(id);
            try
            {
                var result = await asyncOp.ToUniTask();
                return result;
            }
            catch
            {
                _logger.LogWarning($"Can't load asset at path {id}");
                return default;
            }
        }

        public async UniTask<T> Instantiate<T>(string id) where T : Component
        {
            var asyncOp = Addressables.InstantiateAsync(id);
            try
            {
                var result = await asyncOp.ToUniTask();
                if (result.TryGetComponent(out T comp))
                {
                    return comp;
                }
                else
                {
                    return default;
                }
            }
            catch
            {
                _logger.LogWarning($"Can't load asset at path {id}");
                return default;
            }
        }

        public async UniTask<SceneInstance> LoadScene(string id, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            var handle = Addressables.LoadSceneAsync(id, loadSceneMode, activateOnLoad: true);

            while (!handle.IsDone)
            {
                OnProgress?.Invoke(handle.PercentComplete);
                await UniTask.Yield();
            }

            var sceneInstance = handle.Result;
            
            OnProgress?.Invoke(1f);
            return sceneInstance;
        }

        public void UnloadResource<T>(T asset)
        {
            Addressables.Release(asset);
        }

        public async UniTask UnloadScene(SceneInstance scene)
        {
            var asyncOp = Addressables.UnloadSceneAsync(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            try
            {
                await asyncOp.ToUniTask();
            }
            catch
            {
                _logger.LogWarning($"Can't undload scene at path {scene.Scene.path}");
            }
        }
    }
}