using Cysharp.Threading.Tasks;

namespace Core
{
    public class EntryPoint : IAwakable
    {
        private readonly IAsyncGroupLoader _asyncGroupLoader;
        private readonly IGameStateManager _gameStateManager;
        private readonly ILogger _logger;
        private readonly IGameLoop _gameLoop;

        private const string MainGroup = "MainGroup";
        private string parrallel;

        public EntryPoint(IAsyncGroupLoader asyncGroupLoader, IGameStateManager gameStateManager, ILogger logger, IGameLoop gameLoop)
        {
            _asyncGroupLoader = asyncGroupLoader;
            _gameStateManager = gameStateManager;
            _logger = logger;
            _gameLoop = gameLoop;
            
            if (_gameLoop != null)
            {
                _gameLoop.AddToGameLoop(GameLoopType.Awake, this);
            }
        }

        public async void AwakeCustom()
        {
            CreatePlayableGroup();
            await RunGroups();
        }

        private async UniTask RunGroups()
        {
            await _asyncGroupLoader.RunGroup(MainGroup);
        }

        private void CreatePlayableGroup()
        {
            _asyncGroupLoader.CreateGroup(AsyncGroupType.Sequential, MainGroup, true);
            _asyncGroupLoader.AddToGroup(MainGroup, () => _gameStateManager.InitStates());
            _asyncGroupLoader.AddToGroup(MainGroup, () => _gameStateManager.SetState(GameState.MainMenu));
        }
    }
}