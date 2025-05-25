using Cysharp.Threading.Tasks;

namespace Core
{
    public class PauseState : IState
    {
        public GameState Name => GameState.Pause;
        private readonly IGameLoop _gameLoop;
        private readonly IInput _input;
        private readonly ILevelManager _levelManager;
        private readonly ICameraManager _cameraManager;

        public PauseState(IGameLoop gameLoop, ILevelManager levelManager, IInput input, ICameraManager cameraManager)
        {
            _gameLoop = gameLoop;
            _levelManager = levelManager;
            _input = input;
            _cameraManager = cameraManager;
        }

        public async UniTask OnEnter()
        {
            var player = _levelManager.PlayerEntity;
            await _cameraManager.SetActiveCamera(CustomCameraType.UiCamera, player.EyesTransform);
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
            switch (trigger)
            {
                case GameTrigger.QuitToMenu:
                case GameTrigger.GoToMainMenu:
                    return GameState.MainMenu;
                case GameTrigger.Resume:
                    return GameState.Resume;
            }

            return null;
        }
    }
}