using Core;
using Cysharp.Threading.Tasks;

namespace Ui
{
    public class MainMenuModel
    {
        private readonly IGameStateManager _gameStateManager;
        private readonly IGameManager _gameManager;

        public MainMenuModel(IGameStateManager gameStateManager, IGameManager gameManager)
        {
            _gameStateManager = gameStateManager;
            _gameManager = gameManager;
        }

        public async UniTask StartNewGame()
        {
            await _gameStateManager.FireTrigger(GameTrigger.StartGame);
        }

        public async UniTask ExitGame()
        {
            await _gameManager.QuitGame();
        }
    }
}