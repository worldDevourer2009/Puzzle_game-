using Core;
using Cysharp.Threading.Tasks;

namespace Ui
{
    public class StartNewGameModel
    {
        private readonly IGameStateManager _gameStateManager;

        public StartNewGameModel(IGameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        public async UniTask StartNewGame()
        {
            await _gameStateManager.FireTrigger(GameTrigger.StartGame);
        }
    }
}