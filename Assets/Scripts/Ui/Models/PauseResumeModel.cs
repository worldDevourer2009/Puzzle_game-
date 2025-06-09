using System;
using Core;
using R3;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

namespace Ui
{
    public class PauseMenuModel : IDisposable
    {
        private readonly IGameStateManager _gameStateManager;
        
        private readonly ReactiveProperty<GameState> _currentState;
        private readonly CompositeDisposable _compositeDisposable;

        public PauseMenuModel(IGameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
            _compositeDisposable = new CompositeDisposable();
            _currentState = new ReactiveProperty<GameState>();
            
            _gameStateManager.OnGameStateChanged
                .Subscribe(s => _currentState.Value = s)
                .AddTo(_compositeDisposable);
        }

        public async UniTask Pause()
        {
            await _gameStateManager.FireTrigger(GameTrigger.Pause);
        }

        public async UniTask GoToMainMenu()
        {
            await _gameStateManager.FireTrigger(GameTrigger.GoToMainMenu);
        }

        public async UniTask Resume()
        {
            await _gameStateManager.FireTrigger(GameTrigger.Resume);
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}