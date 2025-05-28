using System;
using Core;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using R3;

namespace Ui
{
    public class StartNewGamePresenter : IUIPresenter, IDisposable
    {
        private const string StartNewGameViewId = "StartNewGameView";
        private readonly CompositeDisposable _compositeDisposable;
        
        private readonly StartNewGameModel _startNewGameModel;
        private readonly IFactorySystem _factorySystem;
        private readonly IGameStateManager _gameStateManager;
        private readonly IUISystem _uiSystem;
        
        private IStartNewGameView _startNewGameView;

        public StartNewGamePresenter(StartNewGameModel startNewGameModel, IGameStateManager gameStateManager, IFactorySystem factorySystem, IUISystem uiSystem)
        {
            _startNewGameModel = startNewGameModel;
            _gameStateManager = gameStateManager;
            _factorySystem = factorySystem;
            _uiSystem = uiSystem;

            _compositeDisposable = new CompositeDisposable();
        }

        public async UniTask Initialize()
        {
            _startNewGameView = await _factorySystem.CreateFromInterface<IStartNewGameView>(StartNewGameViewId);
            await _uiSystem.ParentUnderCanvas(_startNewGameView, CanvasType.Windows);
            _startNewGameView.Hide();

            _gameStateManager.OnGameStateChanged.Subscribe(DisplayView)
                .AddTo(_compositeDisposable);
        }

        private void DisplayView(GameState state)
        {
            if (state == GameState.MainMenu)
            {
                _startNewGameView.EnableButton(true);
                _startNewGameView.Show();
            }
            else
            {
                _startNewGameView.Hide();
            }
        }

        public async UniTask OnButtonClicked()
        {
            _startNewGameView.EnableButton(false);
            await _startNewGameModel.StartNewGame();
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}