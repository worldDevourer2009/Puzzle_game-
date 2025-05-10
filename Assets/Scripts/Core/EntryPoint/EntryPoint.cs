using Cysharp.Threading.Tasks;

namespace Core.EntryPoint
{
    public class EntryPoint : IAwakable
    {
        private readonly IAsyncGroupLoader _asyncGroupLoader;
        private readonly ILogger _logger;
        private readonly IGameLoop _gameLoop;

        private string sequentId1;
        private string parrallel;

        public EntryPoint(IAsyncGroupLoader asyncGroupLoader, ILogger logger, IGameLoop gameLoop)
        {
            _asyncGroupLoader = asyncGroupLoader;
            _logger = logger;
            _gameLoop = gameLoop;

            if (_gameLoop != null)
            {
                _gameLoop.AddToGameLoop(GameLoopType.Awake, this);
            }
        }

        public void AwakeCustom()
        {
            // sequentId1 = "sequentional group";
            // _asyncGroupLoader.CreateGroup(AsyncGroupType.Sequential, sequentId1);
            // AddGroupSequent(sequentId1);
            //
            // parrallel = "parrallel group";
            // _asyncGroupLoader.CreateGroup(AsyncGroupType.Parallel, parrallel);
            // AddGroupParallel(parrallel);
            //
            // await RunGroups();
        }

        private void AddGroupParallel(string sequentId)
        {
            _asyncGroupLoader.AddToGroup(sequentId, () => UniTask.WaitForSeconds(3f)
                .ContinueWith(() => _logger.Log("Delay after 3 sec")), false);
            
            _asyncGroupLoader.AddToGroup(sequentId, () => UniTask.WaitForSeconds(4f)
                .ContinueWith(() => _logger.Log("Delay after 4 sec")), false);
            
            _asyncGroupLoader.AddToGroup(sequentId, () => UniTask.WaitForSeconds(5f)
                .ContinueWith(() => _logger.Log("Delay after 5 sec")), false);
        }

        private async UniTask RunGroups()
        {
            await _asyncGroupLoader.RunGroup(sequentId1);

            await UniTask.WaitForSeconds(3f);
            
            await _asyncGroupLoader.RunGroup(parrallel);
        }

        private void AddGroupSequent(string sequentId)
        {
            _asyncGroupLoader.AddToGroup(sequentId, () => UniTask.WaitForSeconds(3f)
                .ContinueWith(() => _logger.Log("Delay after 3 sec")));
            
            _asyncGroupLoader.AddToGroup(sequentId, () => UniTask.WaitForSeconds(4f)
                .ContinueWith(() => _logger.Log("Delay after 4 sec")));
            
            _asyncGroupLoader.AddToGroup(sequentId, () => UniTask.WaitForSeconds(5f)
                .ContinueWith(() => _logger.Log("Delay after 5 sec")));
        }
    }
}