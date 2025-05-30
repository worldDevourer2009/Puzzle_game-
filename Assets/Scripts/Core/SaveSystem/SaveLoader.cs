using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Core
{
    public interface ILoader
    {
        UniTask<GameData> Load(string saveName);
        void Delete(string saveName);
        List<string> ListSaves();
    }
    
    public class SaveLoader : ILoader
    {
        private const string SaveName = "New Save";
        private readonly ISaveSystem _saveSystem;

        public SaveLoader(ISaveSystem saveSystem)
        {
            _saveSystem = saveSystem;
        }

        public async UniTask<GameData> Load(string saveName)
        {
            if (string.IsNullOrEmpty(saveName))
            {
                Logger.Instance.LogWarning("Can't load save with name, loading default");
                var gd = await _saveSystem.Load(SaveName);
                return gd;
            }

            var data = await _saveSystem.Load(saveName);
            return data;
        }

        public void Delete(string saveName)
        {
            if (string.IsNullOrEmpty(saveName))
            {
                Logger.Instance.LogWarning($"There is no save with name {saveName}");
                return;
            }

            _saveSystem.DeleteSave(saveName);
        }
        
        public List<string> ListSaves()
        {
            return _saveSystem.ListSaves();
        }
    }
}