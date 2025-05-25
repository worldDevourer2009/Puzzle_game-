using Cysharp.Threading.Tasks;

namespace Core
{
    public class EntryPoint : IAwakable, IPrioritized
    {
        public int Priority => 1;
        
        private readonly IAsyncGroupLoader _asyncGroupLoader;
        private readonly IGameStateManager _gameStateManager;
        private readonly IAudioSystem _audioSystem;

        private const string MainGroup = "MainGroup";
        private string parrallel;

        public EntryPoint(IAsyncGroupLoader asyncGroupLoader, IGameStateManager gameStateManager, IAudioSystem audioSystem)
        {
            _asyncGroupLoader = asyncGroupLoader;
            _gameStateManager = gameStateManager;
            _audioSystem = audioSystem;
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
            _asyncGroupLoader.CreateGroup(AsyncGroupType.Sequential, MainGroup);
            
            _asyncGroupLoader.AddToGroup(MainGroup, () => _gameStateManager.InitStates());
            _asyncGroupLoader.AddToGroup(MainGroup, () => _audioSystem.InitAudioSystem());
            _asyncGroupLoader.AddToGroup(MainGroup, () => _gameStateManager.SetState(GameState.MainMenu));
        }
    }
}