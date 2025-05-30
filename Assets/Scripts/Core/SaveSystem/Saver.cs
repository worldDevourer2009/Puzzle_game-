using System;
using Cysharp.Threading.Tasks;

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
        private const string SaveName = "New Save";
        
        private readonly ISaveSystem _saveSystem;
        private readonly IPlayerDataHolder _playerDataHolder;
        private readonly IExposedSystemDataHolder _exposedSystemDataHolder;
        private readonly IInputDataHolder _inputDataHolder;

        public Saver(ISaveSystem saveSystem, IPlayerDataHolder playerDataHolder,
            IExposedSystemDataHolder exposedSystemDataHolder, IInputDataHolder inputDataHolder)
        {
            _saveSystem = saveSystem;
            _playerDataHolder = playerDataHolder;
            _exposedSystemDataHolder = exposedSystemDataHolder;
            _inputDataHolder = inputDataHolder;
        }

        public async UniTask SaveAll()
        {
            var data = await _saveSystem.Load(SaveName) ?? new GameData();

            var (stats, interaction) = await _playerDataHolder.WriteData();

            data.PlayerStats = stats;
            data.PlayerInteraction = interaction;
            data.GDName = SaveName;
            data.SaveTimeUtc = DateTime.UtcNow;

            data.SystemData = await _exposedSystemDataHolder.WriteData();
            data.InputData = await _inputDataHolder.WriteData();

            _saveSystem.Save(data);
        }

        public async UniTask SaveProgressAsync()
        {
            var data = await _saveSystem.Load(SaveName) ?? new GameData();

            var playerSettings = await _playerDataHolder.WriteData();

            data.PlayerStats = playerSettings.Item1;
            data.PlayerInteraction = playerSettings.Item2;

            data.GDName = SaveName;
            data.SaveTimeUtc = DateTime.UtcNow;
            _saveSystem.Save(data);
        }

        public async UniTask SaveSettings()
        {
            var data = await _saveSystem.Load(SaveName) ?? new GameData();

            var systemData = await _exposedSystemDataHolder.WriteData();
            data.SystemData = systemData;
            data.InputData = await _inputDataHolder.WriteData();

            data.GDName = SaveName;
            data.SaveTimeUtc = DateTime.UtcNow;

            _saveSystem.Save(data);
        }
    }
}