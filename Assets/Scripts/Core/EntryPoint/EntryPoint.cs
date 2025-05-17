using Cysharp.Threading.Tasks;

namespace Core
{
    public class EntryPoint : IAwakable
    {
        private readonly IAsyncGroupLoader _asyncGroupLoader;
        private readonly ILevelManager _levelManager;
        private readonly ILogger _logger;
        private readonly IGameLoop _gameLoop;

        private const string MainGroup = "MainGroup";
        private string parrallel;

        public EntryPoint(IAsyncGroupLoader asyncGroupLoader, ILevelManager levelManager, ILogger logger, IGameLoop gameLoop)
        {
            _asyncGroupLoader = asyncGroupLoader;
            _levelManager = levelManager;
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
            _asyncGroupLoader.AddToGroup(MainGroup, () => _levelManager.LoadLevelByIndex(1));
        }
    }
}