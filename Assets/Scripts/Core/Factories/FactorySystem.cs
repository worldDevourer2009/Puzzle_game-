using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using Zenject;

namespace Core
{
    public interface IFactorySystem
    {
        UniTask<GameObject> Create(string id);
        UniTask<GameObject> Create(string id, Vector3 position);
        UniTask<GameObject> Create(string id, Transform parent);
        UniTask<GameObject> Create(string id, Vector3 position, Transform parent);
        UniTask<GameObject> CreateGPU(string id, InstanceData data, MeshFilter meshFilter, Material mat);
        void Release(string id, GameObject obj);
        void Inject<T>(T objToInject);
    }
    
    public class FactorySystem : IFactorySystem
    {
        private readonly IPoolSystem _poolSystem;
        private readonly DiContainer _diContainer;

        public FactorySystem(IPoolSystem poolSystem, DiContainer diContainer)
        {
            _poolSystem = poolSystem;
            _diContainer = diContainer;
        }

        public async UniTask<GameObject> Create(string id)
        {
            var asyncOp = _poolSystem.GetObject(id);
            var result = await asyncOp;
            _diContainer.Inject(result);
            return result;
        }

        public async UniTask<GameObject> Create(string id, Vector3 position)
        {
            var asyncOp = _poolSystem.GetObject(id);
            var result = await asyncOp;
            result.transform.position = position;
            _diContainer.Inject(result);
            return result;
        }

        public async UniTask<GameObject> Create(string id, Transform parent)
        {
            var asyncOp = _poolSystem.GetObject(id);
            var result = await asyncOp;
            result.transform.SetParent(parent);
            _diContainer.Inject(result);
            return result;
        }

        public async UniTask<GameObject> Create(string id, Vector3 position, Transform parent)
        {
            var asyncOp = _poolSystem.GetObject(id);
            var result = await asyncOp;
            result.transform.SetParent(parent);
            result.transform.position = position;
            _diContainer.Inject(result);
            return result;
        }

        public async UniTask<GameObject> CreateGPU(string id, InstanceData data, MeshFilter meshFilter, Material mat)
        {
            mat.enableInstancing = true;
            var obj = await _poolSystem.GetObject(id);
            
            var renderParams = new RenderParams(mat)
            {
                worldBounds = new Bounds(obj.transform.position, Vector3.one * 10f)
            };
            
            var mesh = meshFilter.sharedMesh;
            
            if (mesh == null)
            {
                return obj;
            }
            
            data.Matrix4X4 = obj.transform.localToWorldMatrix; 
            var instances = new NativeArray<InstanceData>(1, Allocator.Temp);
            instances[0] = data;
            
            Graphics.RenderMeshInstanced(renderParams, mesh, 0, instances, 1, 0);
            instances.Dispose();

            return obj;
        }

        public void Release(string id, GameObject obj)
        {
            _poolSystem.ReleaseObject(id, obj);
        }

        public void Inject<T>(T objToInject)
        {
            _diContainer.Inject(objToInject);
        }
    }
    
    public struct InstanceData
    {
        public Matrix4x4 Matrix4X4;
        public uint RenderLayerMask;
        public float RenderDistance;
    }
}