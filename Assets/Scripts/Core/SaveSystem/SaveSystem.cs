using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public enum SaveFormat
    {
        Json,
    }
    
    public interface ISaveSystem
    {
        void Save(GameData data, bool overrideSave = true);
        void DeleteSave(string saveName);
        UniTask<GameData> Load(string saveName);
        string GetPath(string fileName);
        List<string> ListSaves();
        DateTime? GetLastSaveTime(string saveName);
    }

    [Serializable]
    public class GameData
    {
        public string GDName;
        public int LevelIndex = 1;
        public Vector3 PlayerPos;
        public PlayerStats PlayerStats;
        public PlayerInteraction PlayerInteraction;
        public LevelData LevelData;
        public DateTime SaveTimeUtc;
        public List<ExposedSystemParameter> SystemData;
        public InputData InputData;
        public bool IsFirstPlay;
    }

    public sealed class SaveSystem : ISaveSystem
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
            var name = data.GDName;

            if (string.IsNullOrEmpty(name))
            {
                name = DefaultSaveName;
            }
            
            var path = GetPath(name);

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

        public List<string> ListSaves()
        {
            if (!Directory.Exists(_persistentPath))
            {
                return new List<string>();
            }
            
            var files = Directory.GetFiles(_persistentPath, "*" + _extension);
            var result = new List<string>(files.Length);
            
            foreach (var file in files)
            {
                result.Add(Path.GetFileNameWithoutExtension(file));
            }
            
            return result;
        }

        public DateTime? GetLastSaveTime(string saveName)
        {
            var path = GetPath(saveName);
            
            if (!File.Exists(path))
            {
                return null;
            }
            
            return File.GetLastWriteTimeUtc(path);
        }

        public UniTask<GameData> Load(string saveName)
        {
            var path = GetPath(saveName);

            if (!File.Exists(path))
            {
                _logger.LogWarning($"File doesn't exist with name {saveName}");
                return UniTask.FromResult(new GameData());
            }

            return UniTask.FromResult(_jsonSerializer.Deserialize<GameData>(File.ReadAllText(path)));
        }

        public string GetPath(string fileName)
        {
            return Path.Combine(_persistentPath, string.Concat(fileName, _extension));
        }
    }
}