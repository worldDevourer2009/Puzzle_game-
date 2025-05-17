using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public enum LoadMode 
{
    Additive,
    Single
}

namespace Core
{
    public class LoadSceneCommand : ISceneCommand
    {
        public string SceneName => _sceneName;
        private readonly string _sceneName;
        private readonly LoadMode _loadMode;
        private readonly ISceneLoader _sceneLoader;

        public LoadSceneCommand(string sceneName, ISceneLoader sceneLoader, LoadMode loadMode)
        {
            _sceneName = sceneName;
            _sceneLoader = sceneLoader;
            _loadMode = loadMode;
        }

        public async UniTask ExecuteAsync()
        {
            LoadSceneMode type;

            switch (_loadMode)
            {
                case LoadMode.Additive:
                    type = LoadSceneMode.Additive;
                    break;
                case LoadMode.Single:
                    type = LoadSceneMode.Single;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            await _sceneLoader.LoadSceneById(_sceneName, type);
        }
    }
}