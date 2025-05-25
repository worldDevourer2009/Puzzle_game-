using System.Collections.Generic;
using ZLinq;

namespace Core
{
    public interface IGameStateFactory
    {
        IState GetState(GameState state);
    }
    
    public class GameStateFactory : IGameStateFactory
    {
        private readonly Dictionary<GameState, IState> _states;
        private readonly ILogger _logger;

        public GameStateFactory(IEnumerable<IState> allStates, ILogger logger)
        {
            _logger = logger;
            _states = new Dictionary<GameState, IState>();
            
            var states = allStates.AsValueEnumerable().ToList();
            
            if (states.AsValueEnumerable().Count() <= 0)
            {
                return;
            }
            
            foreach (var state in states)
            {
                if (!_states.TryAdd(state.Name, state))
                {
                    _logger.LogError($"Duplicate state {state.Name}");
                }
            }
        }
        
        public IState GetState(GameState state)
        {
            if (!_states.TryGetValue(state, out var instance))
            {
                _logger.LogWarning($"State was not registred in the factory with name {state}");
                return null;
            }
            
            return instance;
        }
    }
}