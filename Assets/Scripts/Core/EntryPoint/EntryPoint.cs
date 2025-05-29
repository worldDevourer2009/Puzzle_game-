using Ui;
using UnityEngine;
using Zenject;

namespace Core
{
    public class EntryPoint : MonoBehaviour
    {
        private IAsyncGroupLoader _asyncGroupLoader;
        private IGameStateManager _gameStateManager;
        private IUISystem _uiSystem;
        private IAudioSystem _audioSystem;
        private IAudioManager _audioManager;
        private ICameraManager _cameraManager;
        private UIInitializer _uiInitializer;
        
        private bool _initialized;

        private const string MainGroup = "MainGroup";

        [Inject]
        public void Construct(IAsyncGroupLoader asyncGroupLoader, IGameStateManager gameStateManager, IUISystem uiSystem,
            IAudioSystem audioSystem, IAudioManager audioManager, ICameraManager cameraManager, UIInitializer uiInitializer)
        {
            _asyncGroupLoader = asyncGroupLoader;
            _gameStateManager = gameStateManager;
            _uiSystem = uiSystem;
            _audioSystem = audioSystem;
            _audioManager = audioManager;
            _cameraManager = cameraManager;
            _uiInitializer = uiInitializer;
        }

        private async void Start()
        {
            CreatePlayableGroup();
            await _asyncGroupLoader.RunGroup(MainGroup);
        }

        private void CreatePlayableGroup()
        {
            _asyncGroupLoader.CreateGroup(AsyncGroupType.Sequential, MainGroup);
            _asyncGroupLoader.AddToGroup(MainGroup, () => _gameStateManager.InitStates());
            _asyncGroupLoader.AddToGroup(MainGroup, () => _uiSystem.InitializeUiSystem());
            _asyncGroupLoader.AddToGroup(MainGroup, () => _audioSystem.InitAudioSystem());
            _asyncGroupLoader.AddToGroup(MainGroup, () => _audioManager.InitAudioManager());
            _asyncGroupLoader.AddToGroup(MainGroup, () => _uiInitializer.InitializeUI());
            _asyncGroupLoader.AddToGroup(MainGroup, () => _cameraManager.CreateCamera(CustomCameraType.BackupCamera));
            _asyncGroupLoader.AddToGroup(MainGroup, () => _gameStateManager.SetState(GameState.MainMenu));
        }
    }
}