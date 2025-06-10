using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using Zenject;

namespace Core
{
    public interface IFactorySystem
    {
        UniTask<GameObject> Create(string id);
        UniTask<T> Create<T>(string id) where T : Component;
        UniTask<T> Create<T>(string id, Vector3 position) where T : Component;
        UniTask<T> Create<T>(string id, Transform parent) where T : Component;
        UniTask<T> Create<T>(string id, Vector3 position, Transform parent) where T : Component;
        UniTask<GameObject> Create(string id, Vector3 position);
        UniTask<GameObject> Create(string id, Transform parent);
        UniTask<GameObject> Create(string id, Vector3 position, Transform parent);
        UniTask<GameObject> CreateGPU(string id, InstanceData data, MeshFilter meshFilter, Material mat);
        UniTask<TInterface> CreateFromInterface<TInterface>(string id) where TInterface : class;
        UniTask<TInterface> CreateFromInterface<TInterface>(string id, Vector3 position) where TInterface : class;
        UniTask<TInterface> CreateFromInterface<TInterface>(string id, Transform parent) where TInterface : class;
        UniTask<TInterface> CreateFromInterface<TInterface>(string id, Vector3 position, Transform parent) where TInterface : class;
        void Release(string id, GameObject obj);
        void Inject<T>(T objToInject);
    }

    public class FactorySystem : IFactorySystem
    {
        private readonly IPoolSystem _poolSystem;
        private readonly IContextResolver _diContainer;

        public FactorySystem(IPoolSystem poolSystem, IContextResolver diContainer)
        {
            _poolSystem = poolSystem;
            _diContainer = diContainer;
        }

        public async UniTask<GameObject> Create(string id)
        {
            var (result, container) = await InternalCreate(id);
            container.InjectGameObject(result);
            return result;
        }

        public async UniTask<T> Create<T>(string id) where T : Component
        {
            var (result, container) = await InternalCreate<T>(id);
            container.Inject(result);
            return result;
        }

        public async UniTask<T> Create<T>(string id, Vector3 position) where T : Component
        {
            var (result, container) = await InternalCreate<T>(id);
            container.Inject(result);
            result.transform.position = position;
            return result;
        }

        public async UniTask<T> Create<T>(string id, Transform parent) where T : Component
        {
            var (result, container) = await InternalCreate<T>(id);
            container.Inject(result);
            
            if (parent != null)
            {
                result.transform.SetParent(parent);
            }

            return result;
        }

        public async UniTask<T> Create<T>(string id, Vector3 position, Transform parent) where T : Component
        {
            var (result, container) = await InternalCreate<T>(id);
            container.Inject(result);

            if (parent != null)
            {
                result.transform.position = position;
                result.transform.SetParent(parent);
            }

            return result;
        }

        public async UniTask<GameObject> Create(string id, Vector3 position)
        {
            var (result, container) = await InternalCreate(id);
            result.transform.position = position;
            container.InjectGameObject(result);
            return result;
        }

        public async UniTask<GameObject> Create(string id, Transform parent)
        {
            var (result, container) = await InternalCreate(id);
            result.transform.SetParent(parent);
            container.InjectGameObject(result);
            return result;
        }

        public async UniTask<GameObject> Create(string id, Vector3 position, Transform parent)
        {
            var (result, container) = await InternalCreate(id);
            result.transform.SetParent(parent, worldPositionStays: true);
            result.transform.position = position;
            container.InjectGameObject(result);
            return result;
        }
        
        public async UniTask<TInterface> CreateFromInterface<TInterface>(string id) where TInterface : class
        {
            var (go, container) = await InternalCreate(id);
            container.InjectGameObject(go);
            var inst = go.GetComponent<TInterface>();
            if (inst == null)
                Debug.LogError($"Prefab '{id}' не содержит компонент, реализующий {typeof(TInterface).Name}");
            return inst;
        }

        public async UniTask<TInterface> CreateFromInterface<TInterface>(string id, Vector3 position) where TInterface : class
        {
            var inst = await CreateFromInterface<TInterface>(id);
            if (inst is Component c) c.transform.position = position;
            return inst;
        }

        public async UniTask<TInterface> CreateFromInterface<TInterface>(string id, Transform parent) where TInterface : class
        {
            var inst = await CreateFromInterface<TInterface>(id);
            if (inst is Component c && parent != null) c.transform.SetParent(parent);
            return inst;
        }

        public async UniTask<TInterface> CreateFromInterface<TInterface>(string id, Vector3 position, Transform parent) where TInterface : class
        {
            var inst = await CreateFromInterface<TInterface>(id);
            if (inst is Component c && parent != null)
            {
                c.transform.position = position;
                c.transform.SetParent(parent);
            }
            return inst;
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
            var context = _diContainer.ResolveFor(objToInject);
            context.Inject(objToInject);
        }

        private async UniTask<(GameObject result, DiContainer context)> InternalCreate(string id)
        {
            var asyncOp = _poolSystem.GetObject(id);
            var result = await asyncOp;
            var context = _diContainer.ResolveFor(result);
            return (result, context);
        }

        private async UniTask<(T result, DiContainer context)> InternalCreate<T>(string id) where T : Component
        {
            var asyncOp = _poolSystem.GetObject<T>(id);
            var result = await asyncOp;
            var context = _diContainer.ResolveFor(result);
            return (result, context);
        }
    }

    public struct InstanceData
    {
        public Matrix4x4 Matrix4X4;
        public uint RenderLayerMask;
        public float RenderDistance;
    }
}