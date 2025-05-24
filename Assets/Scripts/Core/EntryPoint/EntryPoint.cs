using Cysharp.Threading.Tasks;

namespace Core
{
    public class EntryPoint : IAwakable
    {
        private readonly IAsyncGroupLoader _asyncGroupLoader;
        private readonly IGameStateManager _gameStateManager;
        private readonly ILogger _logger;

        private const string MainGroup = "MainGroup";
        private string parrallel;

        public EntryPoint(IAsyncGroupLoader asyncGroupLoader, IGameStateManager gameStateManager, ILogger logger)
        {
            _asyncGroupLoader = asyncGroupLoader;
            _gameStateManager = gameStateManager;
            _logger = logger;
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