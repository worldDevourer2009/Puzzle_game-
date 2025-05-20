using Cysharp.Threading.Tasks;

namespace Core
{
    public class PauseState : IState
    {
        public GameState Name => GameState.Pause;
        private readonly IGameLoop _gameLoop;
        private readonly IInput _input;
        private readonly ICameraManager _cameraManager;

        public PauseState(IGameLoop gameLoop, IInput input, ICameraManager cameraManager)
        {
            _gameLoop = gameLoop;
            _input = input;
            _cameraManager = cameraManager;
        }

        public async UniTask OnEnter()
        {
            await _cameraManager.SetActiveCamera(CustomCameraType.UiCamera);
            _gameLoop.EnableUpdate(false);
            _input.EnableInput(false);
        }

        public UniTask OnUpdate()
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnExit()
        {
            return UniTask.CompletedTask;
        }

        public GameState? NextState(GameTrigger trigger)
        {
            return trigger switch
            {
                GameTrigger.Resume => GameState.Resume,
                GameTrigger.QuitToMenu => GameState.MainMenu,
                _ => null
            };
        }
    }
}