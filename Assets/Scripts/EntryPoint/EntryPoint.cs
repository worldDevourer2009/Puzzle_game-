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

        private IPlayerDataHolder _playerDataHolder;
        private IExposedSystemDataHolder _exposedSystemDataHolder;
        private IInputDataHolder _inputDataHolder;

        private bool _initialized;

        private const string MainGroup = "MainGroup";
        private const string DataGroup = "DataGroup";

        [Inject]
        public void Construct(IAsyncGroupLoader asyncGroupLoader, IGameStateManager gameStateManager, IUISystem uiSystem,
            IAudioSystem audioSystem, IAudioManager audioManager, ICameraManager cameraManager,
            UIInitializer uiInitializer, IPlayerDataHolder playerDataHolder,
            IExposedSystemDataHolder exposedSystemDataHolder, IInputDataHolder inputDataHolder)
        {
            _asyncGroupLoader = asyncGroupLoader;
            _gameStateManager = gameStateManager;
            _uiSystem = uiSystem;
            _audioSystem = audioSystem;
            _audioManager = audioManager;
            _cameraManager = cameraManager;
            _uiInitializer = uiInitializer;
            _playerDataHolder = playerDataHolder;
            _exposedSystemDataHolder = exposedSystemDataHolder;
            _inputDataHolder = inputDataHolder;
            
            CreateMainGroup();
            CreateDataGroup();
        }

        private async void Start()
        {
            if (_initialized)
            {
                return;
            }
            
            await _asyncGroupLoader.RunGroup(DataGroup);
            await _cameraManager.CreateCamera(CustomCameraType.BackupCamera);
            await _asyncGroupLoader.RunGroup(MainGroup);
            _initialized = true;
        }

        private void CreateMainGroup()
        {
            _asyncGroupLoader.CreateGroup(AsyncGroupType.Sequential, MainGroup);

            _asyncGroupLoader.AddToGroup(MainGroup, () => _gameStateManager.InitStates());
            _asyncGroupLoader.AddToGroup(MainGroup, () => _uiSystem.InitializeUiSystem());
            _asyncGroupLoader.AddToGroup(MainGroup, () => _audioSystem.InitAudioSystem());
            _asyncGroupLoader.AddToGroup(MainGroup, () => _audioManager.InitAudioManager());
            _asyncGroupLoader.AddToGroup(MainGroup, () => _uiInitializer.InitializeUI());
            _asyncGroupLoader.AddToGroup(MainGroup, () => _gameStateManager.SetState(GameState.MainMenu));
        }

        private void CreateDataGroup()
        {
            _asyncGroupLoader.CreateGroup(AsyncGroupType.Parallel, DataGroup);

            _asyncGroupLoader.AddToGroup(DataGroup, () => _playerDataHolder.InitData(), false);
            _asyncGroupLoader.AddToGroup(DataGroup, () => _exposedSystemDataHolder.InitData(), false);
            _asyncGroupLoader.AddToGroup(DataGroup, () => _inputDataHolder.InitData(), false);
        }
    }
}