using Cysharp.Threading.Tasks;

namespace Ui
{
    public class StartNewGamePresenter
    {
        private readonly StartNewGameModel _startNewGameModel;
        private readonly IStartNewGameView _startNewGameView;

        public StartNewGamePresenter(StartNewGameModel startNewGameModel, IStartNewGameView startNewGameView)
        {
            _startNewGameModel = startNewGameModel;
            _startNewGameView = startNewGameView;
        }

        public async UniTask OnButtonClicked()
        {
            _startNewGameView.DisableButton();
            await _startNewGameModel.StartNewGame();
        }
    }
}