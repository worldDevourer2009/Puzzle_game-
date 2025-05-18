using System;
using Core;
using R3;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

namespace Ui
{
    public class PauseResumeModel : IDisposable
    {
        public ReactiveProperty<GameState> CurrentState => _currentState; 
        private readonly IGameStateManager _gameStateManager;
        private readonly IInput _input;
        
        private readonly ReactiveProperty<GameState> _currentState;
        private readonly CompositeDisposable _compositeDisposable;

        public PauseResumeModel(IGameStateManager gameStateManager, IInput input)
        {
            _gameStateManager = gameStateManager;
            _input = input;
            _compositeDisposable = new CompositeDisposable();
            _currentState = new ReactiveProperty<GameState>();
            
            _gameStateManager
                .OnGameStateChanged
                .Subscribe(OnGameStateChanged)
                .AddTo(_compositeDisposable);

            _input.OnPauseClicked += HandleClick;
        }

        private void HandleClick()
        {
            var currentState = _gameStateManager.CurrentState;
            
            if (currentState == GameState.Pause)
            {
                OnGameStateChanged(GameState.Resume).Forget();
            }
            else
            {
                OnGameStateChanged(GameState.Pause).Forget();
            }
        }

        private async UniTaskVoid OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Pause:
                    await Pause();
                    break;
                case GameState.Resume:
                    await Resume();
                    break;
            }
        }

        public async UniTask Pause()
        {
            await _gameStateManager.FireTrigger(GameTrigger.Pause);
            _currentState.Value = GameState.Pause;
        }

        public async UniTask Resume()
        {
            await _gameStateManager.FireTrigger(GameTrigger.Resume);
            _currentState.Value = GameState.Resume;
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}