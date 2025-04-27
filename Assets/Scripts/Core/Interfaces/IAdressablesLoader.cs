using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core
{
    public interface IAddressablesLoader
    {
        UniTask LoadGameObject<T>(string id);
    }

    public class AddressablesLoader : IAddressablesLoader
    {
        public async UniTask LoadGameObject<T>(string id)
        {
            var ga = Addressables.LoadAssetAsync<T>(id);
            await ga.Task.AsUniTask();
        }
    }
}