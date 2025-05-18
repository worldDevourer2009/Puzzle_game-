using Cysharp.Threading.Tasks;

namespace Core
{
    public class MainMenuState : IState
    {
        public GameState Name => GameState.MainMenu;
        
        private readonly IGameManager _gameManager;
        private readonly IGameLoop _gameLoop;

        public MainMenuState(IGameManager gameManager, IGameLoop gameLoop)
        {
            _gameManager = gameManager;
            _gameLoop = gameLoop;
        }

        public async UniTask OnEnter()
        {
            await _gameManager.LaunchGame();
            _gameLoop.EnableUpdate(false);
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
            return trigger == GameTrigger.StartGame 
                ? GameState.NewGame 
                : null;
        }
    }
}