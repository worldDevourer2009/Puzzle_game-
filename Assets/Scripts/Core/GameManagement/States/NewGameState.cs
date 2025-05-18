using Cysharp.Threading.Tasks;

namespace Core
{
    public class NewGameState : IState
    {
        public GameState Name => GameState.NewGame;
        private readonly IGameManager _gameManager;
        private readonly IGameLoop _gameLoop;

        public NewGameState(IGameManager gameManager, IGameLoop gameLoop)
        {
            _gameManager = gameManager;
            _gameLoop = gameLoop;
        }

        public async UniTask OnEnter()
        {
            await _gameManager.StartNewGame();
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
                GameTrigger.Resume => GameState.Resume,
                _ => null
            };
        }
    }
}