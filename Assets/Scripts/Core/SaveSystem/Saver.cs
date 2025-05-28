using System;
using Cysharp.Threading.Tasks;
using Game;

namespace Core
{
    public interface ISaver
    {
        UniTask SaveAll();
        UniTask SaveProgressAsync();
        UniTask SaveSettings();
    }
    
    public class Saver : ISaver
    {
        private readonly ISaveSystem _saveSystem;
        private readonly IPlayerDataHolder _playerDataHolder;
        private readonly string _saveName = "New Save";

        public Saver(ISaveSystem saveSystem, IPlayerDataHolder playerDataHolder)
        {
            _saveSystem = saveSystem;
            _playerDataHolder = playerDataHolder;
        }

        public async UniTask SaveAll()
        {
            await SaveProgressAsync();
            await SaveSettings();
        }

        public async UniTask SaveProgressAsync()
        {
            var data = await _saveSystem.Load(_saveName) ?? new GameData();
            
            var playerSettings = await _playerDataHolder.WriteData();
            
            data.PlayerStats = playerSettings.Item1;
            data.PlayerInteraction = playerSettings.Item2;
            
            data.GDName = _saveName;
            data.SaveTimeUtc = DateTime.UtcNow;
            _saveSystem.Save(data);
        }

        public async UniTask SaveSettings()
        {
            var data = await _saveSystem.Load(_saveName) ?? new GameData();
        }
    }
}