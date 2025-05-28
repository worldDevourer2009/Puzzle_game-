using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public enum GameState
    {
        Default,
        MainMenu,
        NewGame,
        LoadGame,
        Playing,
        Pause,
        Resume
    }

    public enum GameTrigger
    {
        GoToMainMenu,
        StartGame,
        Pause,
        Resume,
        QuitToMenu
    }

    public interface IGameStateManager
    {
        AsyncReactiveProperty<GameState> OnGameStateChanged { get; }
        GameState CurrentState { get; }
        UniTask InitStates();
        UniTask SetState(GameState nextState);
        UniTask FireTrigger(GameTrigger trigger);
    }

    public class GameStateManager : IGameStateManager
    {
        public AsyncReactiveProperty<GameState> OnGameStateChanged => _state;
        public GameState CurrentState => _currentState.Name;

        private readonly IGameStateFactory _gameStateFactory;
        private readonly ILogger _logger;

        private readonly AsyncReactiveProperty<GameState> _state;
        private readonly Dictionary<GameState, IState> _states;

        private IState _currentState;

        public GameStateManager(IGameStateFactory gameStateFactory, ILogger logger)
        {
            _gameStateFactory = gameStateFactory;
            _logger = logger;

            _states = new Dictionary<GameState, IState>();
            _state = new(GameState.Default);
        }

        public UniTask InitStates()
        {
            var states = Enum.GetValues(typeof(GameState))
                .Cast<GameState>()
                .ToList();

            foreach (var state in states)
            {
                if (state == GameState.Default)
                {
                    continue;
                }
                
                var newState = _gameStateFactory.GetState(state);

                if (newState == null)
                {
                    _logger.Log($"Can't create state {state}");
                    continue;
                }
                
                _states[newState.Name] = newState;
            }

            _state.Value = GameState.MainMenu;

            return UniTask.CompletedTask;
        }

        public async UniTask SetState(GameState nextState)
        {
            if (_currentState != null)
            {
                await _currentState.OnExit();
            }

            if (_states.TryGetValue(nextState, out var state))
            {
                await state.OnEnter();
                await state.OnUpdate();
                
                _currentState = state;
                _state.Value = state.Name;
            }
            else
            {
                _logger.Log($"Can't get state {nextState}");
            }
        }

        public async UniTask FireTrigger(GameTrigger trigger)
        {
            if (_currentState == null)
            {
                _logger.LogWarning("State machine not initialized");
                return;
            }

            var next = _currentState.NextState(trigger);

            if (next == null || next.Value == _currentState.Name)
            {
                return;
            }

            await SetState(next.Value);
        }
    }
}