using Cysharp.Threading.Tasks;

namespace Core
{
    public class NewGameState : IState
    {
        public GameState Name => GameState.NewGame;
        private readonly IGameManager _gameManager;
        private readonly IAudioSystem _audioSystem;
        private readonly IGameLoop _gameLoop;
        private readonly IInput _input;

        public NewGameState(IGameManager gameManager, IAudioSystem audioSystem, IGameLoop gameLoop, IInput input)
        {
            _gameManager = gameManager;
            _audioSystem = audioSystem;
            _gameLoop = gameLoop;
            _input = input;
        }

        public async UniTask OnEnter()
        {
            await _gameManager.StartNewGame();
            await _audioSystem.PlayMusic(SoundClipId.FirstLevelBackground_1);
            _gameLoop.EnableUpdate(true);
            _input.EnableInput(true);
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