using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public interface IPoolSystem
    {
        UniTask Prewarm(string id, int count, string parentName = null, bool dontDestroyOnLoad = false);

        UniTask Prewarm<T>(string id, int count, string parentName = null, bool dontDestroyOnLoad = false)
            where T : Component;

        UniTask<GameObject> GetObject(string id);
        UniTask<T> GetObject<T>(string id) where T : Component;
        UniTask<TInterface> GetObjectInterface<TInterface>(string id) where TInterface : class;
        void ReleaseObject<T>(string id, T component) where T : Component;
        void ReleaseObject(string id, GameObject obj);
    }

    public class PoolSystem : IPoolSystem
    {
        private readonly IAddressableLoader _addressableLoader;
        private readonly Dictionary<string, Queue<GameObject>> _poolDictionary;
        private readonly Dictionary<string, Queue<Component>> _poolCompDictionary;

        public PoolSystem(IAddressableLoader addressableLoader)
        {
            _addressableLoader = addressableLoader;
            _poolDictionary = new Dictionary<string, Queue<GameObject>>();
            _poolCompDictionary = new Dictionary<string, Queue<Component>>();
        }

        public async UniTask Prewarm(string id, int count, string parentName = null, bool dontDestroyOnLoad = false)
        {
            if (!_poolDictionary.TryGetValue(id, out var queue))
            {
                queue = new Queue<GameObject>();
                _poolDictionary[id] = queue;
            }
            
            await PrewarmInternal<GameObject>(id, count, parentName, dontDestroyOnLoad, queue);
        }

        public async UniTask Prewarm<T>(string id, int count, string parentName = null, bool dontDestroyOnLoad = false)
            where T : Component
        {
            if (!_poolDictionary.TryGetValue(id, out var queue))
            {
                queue = new Queue<GameObject>();
                _poolDictionary[id] = queue;
            }

            await PrewarmInternal<T>(id, count, parentName, dontDestroyOnLoad, queue);
        }

        private async Task PrewarmInternal<T>(string id, int count, string parentName, bool dontDestroyOnLoad, Queue<GameObject> queue)
        {
            var parent = GetOrCreateParent(parentName, dontDestroyOnLoad);

            var toCreate = count - queue.Count;

            for (var i = 0; i < toCreate; i++)
            {
                var obj = await CreateObject(id);

                if (dontDestroyOnLoad)
                {
                    Object.DontDestroyOnLoad(obj);
                }

                if (parent != null)
                {
                    obj.transform.SetParent(parent);
                }

                obj.SetActive(false);
                queue.Enqueue(obj);
            }
        }

        private Transform GetOrCreateParent(string parentName, bool dontDestroyOnLoad)
        {
            var name = string.IsNullOrWhiteSpace(parentName) ? "Pool_Root" : parentName;
            var existing = GameObject.Find(name);

            if (existing != null)
            {
                return existing.transform;
            }

            var go = new GameObject(name);

            if (dontDestroyOnLoad)
            {
                Object.DontDestroyOnLoad(go);
            }

            return go.transform;
        }

        private async UniTask<GameObject> CreateObject(string id)
        {
            var asyncOp = _addressableLoader.Instantiate(id);
            var result = await asyncOp;
            return result;
        }

        public async UniTask<GameObject> GetObject(string id)
        {
            if (!_poolDictionary.TryGetValue(id, out var queue))
            {
                queue = new Queue<GameObject>();
                _poolDictionary[id] = queue;
            }

            if (queue.Count == 0)
            {
                return await CreateObject(id);
            }

            var obj = queue.Dequeue();
            obj.SetActive(true);

            return obj;
        }

        public async UniTask<T> GetObject<T>(string id) where T : Component
        {
            if (!_poolDictionary.TryGetValue(id, out var objQueue))
            {
                objQueue = new Queue<GameObject>();
                _poolDictionary[id] = objQueue;
            }

            GameObject obj;

            if (objQueue.Count == 0)
            {
                obj = await CreateObject(id);
            }
            else
            {
                obj = objQueue.Dequeue();
            }

            obj.SetActive(true);

            var component = obj.GetComponent<T>();
            if (component == null)
            {
                Logger.Instance.LogWarning($"GA does not contain compo");
                return null;
            }

            return component;
        }
        
        public async UniTask<TInterface> GetObjectInterface<TInterface>(string id) where TInterface : class
        {
            var go = await GetObject(id);
            
            var inst = go.GetComponent<TInterface>();
            
            if (inst == null)
            {
                Logger.Instance.LogWarning($"GA does not contain compo");
            }
            
            return inst;
        }

        public void ReleaseObject(string id, GameObject obj)
        {
            if (!_poolDictionary.TryGetValue(id, out var queue))
            {
                queue = new Queue<GameObject>();
                _poolDictionary[id] = queue;
            }

            obj.SetActive(false);
            _poolDictionary[id].Enqueue(obj);
        }

        public void ReleaseObject<T>(string id, T component) where T : Component
        {
            if (component == null)
                return;

            var obj = component.gameObject;
            ReleaseObject(id, obj);
        }
    }
}