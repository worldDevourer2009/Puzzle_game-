using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public interface IFactorySystem
    {
        UniTask<GameObject> Create(string id);
        UniTask<GameObject> Create(string id, Vector3 position);
        UniTask<GameObject> Create(string id, Transform parent);
        UniTask<GameObject> Create(string id, Transform parent, Vector3 position);
        void Release(string id, GameObject obj);
    }
    
    public class FactorySystem : IFactorySystem
    {
        private readonly IPoolSystem _poolSystem;

        public FactorySystem(IPoolSystem poolSystem)
        {
            _poolSystem = poolSystem;
        }

        public async UniTask<GameObject> Create(string id)
        {
            var asyncOp = _poolSystem.GetObject(id);
            var result = await asyncOp;
            return result;
        }

        public async UniTask<GameObject> Create(string id, Vector3 position)
        {
            var asyncOp = _poolSystem.GetObject(id);
            var result = await asyncOp;
            result.transform.position = position;
            return result;
        }

        public async UniTask<GameObject> Create(string id, Transform parent)
        {
            var asyncOp = _poolSystem.GetObject(id);
            var result = await asyncOp;
            result.transform.SetParent(parent);
            return result;
        }

        public async UniTask<GameObject> Create(string id, Transform parent, Vector3 position)
        {
            var asyncOp = _poolSystem.GetObject(id);
            var result = await asyncOp;
            result.transform.SetParent(parent);
            result.transform.position = position;
            return result;
        }

        public void Release(string id, GameObject obj)
        {
            _poolSystem.ReleaseObject(id, obj);
        }
    }
}