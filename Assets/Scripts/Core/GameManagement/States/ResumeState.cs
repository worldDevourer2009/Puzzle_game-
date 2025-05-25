using Cysharp.Threading.Tasks;

namespace Core
{
    public class ResumeState : IState
    {
        public GameState Name => GameState.Resume;
        private readonly IGameLoop _gameLoop;
        private readonly IInput _input;
        private readonly ICameraManager _cameraManager;

        public ResumeState(IGameLoop gameLoop, IInput input, ICameraManager cameraManager)
        {
            _gameLoop = gameLoop;
            _input = input;
            _cameraManager = cameraManager;
        }

        public async UniTask OnEnter()
        {
            await _cameraManager.SetActiveCamera(CustomCameraType.PlayerCamera);
            _input.EnableInput(true);
            _gameLoop.EnableUpdate(true);
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
                GameTrigger.Pause => GameState.Pause,
                _ => null
            };
        }
    }
}