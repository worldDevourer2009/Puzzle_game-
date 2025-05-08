using System;
using System.IO;
using UnityEngine;

namespace Core
{
    public interface ISaveSystem
    {
        void Save(GameData data, bool overrideSave = true);
        void DeleteSave(string saveName);
        GameData Load(string saveName);
        string GetPath(string fileName);
    }

    [Serializable]
    public class GameData
    {
        public string GDName;
        public float StatHealth;
        public PlayerStats PlayerStats;
    }

    [Serializable]
    public struct PlayerStats
    {
        public float Health;
        public float Speed;
        public float JumpForce;
    }

    public class SaveSystem : ISaveSystem
    {
        private const string DefaultSaveName = "New Save";

        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger _logger;

        private readonly string _persistentPath;
        private readonly string _extension;

        public SaveSystem(IJsonSerializer jsonSerializer, ILogger logger)
        {
            _jsonSerializer = jsonSerializer;
            _logger = logger;

            _persistentPath = Application.persistentDataPath;
            _extension = ".json";
        }

        public void Save(GameData data, bool overrideSave = true)
        {
            var path = GetPath(data.GDName);

            if (!overrideSave && File.Exists(path))
            {
                _logger.LogWarning($"File already exists and can't be overwritten with path {path}");
                return;
            }

            File.WriteAllText(path, _jsonSerializer.Serialize<GameData>(data));
        }

        public void DeleteSave(string saveName)
        {
            var path = GetPath(saveName);

            if (!File.Exists(path))
            {
                _logger.LogWarning($"File doesn't exist and can't be deleted with path {path}");
                return;
            }

            File.Delete(path);
        }

        public GameData Load(string saveName)
        {
            var path = GetPath(saveName);

            if (!File.Exists(path))
            {
                _logger.LogWarning($"File doesn't exist with name {saveName}");
                return new GameData();
            }

            return _jsonSerializer.Deserialize<GameData>(File.ReadAllText(path));
        }

        public string GetPath(string fileName)
        {
            return Path.Combine(_persistentPath, string.Concat(fileName, _extension));
        }
    }
}