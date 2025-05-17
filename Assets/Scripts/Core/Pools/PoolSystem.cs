using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public interface IPoolSystem
    {
        UniTask Prewarm(string id, int count);
        UniTask<GameObject> GetObject(string id);
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
        
        public async UniTask Prewarm(string id, int count)
        {
            if (!_poolDictionary.TryGetValue(id, out var queue))
            {
                queue = new Queue<GameObject>();
                _poolDictionary[id] = queue;
            }

            var toCreate = count - queue.Count;

            for (var i = 0; i < toCreate; i++)
            {
                var obj = await CreateObject(id);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
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
    }
}