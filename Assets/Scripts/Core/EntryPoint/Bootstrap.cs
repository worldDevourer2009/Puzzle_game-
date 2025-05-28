using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Core
{
    public static class Bootstrap
    {
        private const string BootstrapSceneId = "Bootstrap";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            LoadBootstrapScene().Forget();
        }
        
        private static async UniTaskVoid LoadBootstrapScene()
        {
            if (SceneManager.GetSceneByName(BootstrapSceneId).isLoaded)
            {
                return;
            }
        
            var handle = Addressables.LoadSceneAsync(BootstrapSceneId);
        
            await handle.ToUniTask();
        }
    }
}