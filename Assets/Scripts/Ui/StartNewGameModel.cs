using Core;
using Cysharp.Threading.Tasks;

namespace Ui
{
    public class StartNewGameModel
    {
        private readonly IGameManager _gameManager;

        public StartNewGameModel(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public async UniTask StartNewGame()
        {
            await _gameManager.StartNewGame();
        }
    }
}