using Cysharp.Threading.Tasks;
using ZLinq;

namespace Core
{
    public class MainMenuState : IState
    {
        public GameState Name => GameState.MainMenu;

        private readonly ISceneLoader _sceneLoader;
        private readonly IGameLoop _gameLoop;
        private readonly ICameraManager _cameraManager;
        private readonly IAudioSystem _audioSystem;
        
        private readonly ScenesConfig _scenesConfig;

        public MainMenuState(ISceneLoader sceneLoader, IGameLoop gameLoop,
            ICameraManager cameraManager, IAudioSystem audioSystem, ScenesConfig scenesConfig)
        {
            _sceneLoader = sceneLoader;
            _gameLoop = gameLoop;
            _cameraManager = cameraManager;
            _audioSystem = audioSystem;
            _scenesConfig = scenesConfig;
        }

        public async UniTask OnEnter()
        {
            var mainMenu = _scenesConfig.Scenes.AsValueEnumerable().FirstOrDefault();
            
            if (string.IsNullOrWhiteSpace(mainMenu))
            {
                Logger.Instance.Log("Can't find main menu scene");
                return;
            }
            
            await _sceneLoader.LoadSceneById(mainMenu);
            _gameLoop.EnableUpdate(false);
            await _cameraManager.CreateCamera(CustomCameraType.UiCamera);
        }

        public async UniTask OnUpdate()
        {
            await _audioSystem.PlayMusic(SoundClipId.MainMenuIntro_1);
        }

        public async UniTask OnExit()
        {
            _audioSystem.StopSound(SoundClipId.MainMenuIntro_1);
            await UniTask.Delay(500);
        }

        public GameState? NextState(GameTrigger trigger)
        {
            return trigger == GameTrigger.StartGame 
                ? GameState.NewGame 
                : null;
        }
    }
}