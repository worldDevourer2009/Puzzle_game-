using System;
using System.Collections.Generic;

namespace Core
{
    public enum GameLoopType
    {
        None,
        Awake,
        Update,
        FixedUpdate,
        LateUpdate
    }

    public interface IGameLoop
    {
        event Action OnAfterInit;
        void AddToGameLoop(GameLoopType loopType, object objToAdd);
        void RemoveFromLoop(GameLoopType loopType, object obj);
        void Awake();
        void Update();
        void FixedUpdate();
        void LateUpdate();
        void EnableUpdate(bool enable);
    }

    public interface IAwakable
    {
        void AwakeCustom();
    }

    public interface IUpdatable
    {
        void UpdateCustom();
    }

    public interface IFixedUpdatable
    {
        void FixedUpdateCustom();
    }

    public interface ILateUpdatable
    {
        void LateUpdateCustom();
    }

    public interface IPrioritized
    {
        int Priority { get; }
    }

    public class GameLoop : IGameLoop
    {
        private readonly ILogger _logger;
        private readonly SortedDictionary<int, List<IAwakable>> _awakeDictionary;
        private readonly Dictionary<IUpdatable, IUpdatable> _updateDictionary;
        private readonly Dictionary<IFixedUpdatable, IFixedUpdatable> _fixedUpdateDictionary;
        private readonly Dictionary<ILateUpdatable, ILateUpdatable> _lateUpdateDictionary;
        private bool _enabled;

        public event Action OnAfterInit;

        public GameLoop(ILogger logger)
        {
            _logger = logger;
            _awakeDictionary = new SortedDictionary<int, List<IAwakable>>();
            _updateDictionary = new Dictionary<IUpdatable, IUpdatable>();
            _fixedUpdateDictionary = new Dictionary<IFixedUpdatable, IFixedUpdatable>();
            _lateUpdateDictionary = new Dictionary<ILateUpdatable, ILateUpdatable>();
            _enabled = true;
        }

        public void AddToGameLoop(GameLoopType loopType, object objToAdd)
        {
            if (objToAdd == null)
            {
                return;
            }
            
            switch (loopType)
            {
                case GameLoopType.Awake:
                    if (objToAdd is IAwakable awakable)
                    {
                        var priority = (objToAdd as IPrioritized)?.Priority ?? 100;
                        
                        if (!_awakeDictionary.TryGetValue(priority, out var list))
                        {
                            list = new List<IAwakable>();
                            _awakeDictionary.Add(priority, list);
                        }
                        list.Add(awakable);
                    }
                    break;
                case GameLoopType.Update:
                    if (objToAdd is IUpdatable updatable)
                    {
                        _updateDictionary.TryAdd(updatable, updatable);
                    }
                    else
                    {
                        _logger.LogWarning($"Type {objToAdd.GetType()} not IUpdatable");
                    }
                    break;
                case GameLoopType.FixedUpdate:
                    if (objToAdd is IFixedUpdatable fixedUpdatable)
                    {
                        _fixedUpdateDictionary.TryAdd(fixedUpdatable, fixedUpdatable);
                    }
                    else
                    {
                        _logger.LogWarning($"Type {objToAdd.GetType()} not IFixedUpdatable");
                    }
                    break;
                case GameLoopType.LateUpdate:
                    if (objToAdd is ILateUpdatable lateUpdatable)
                    {
                        _lateUpdateDictionary.TryAdd(lateUpdatable, lateUpdatable);
                    }
                    else
                    {
                        _logger.LogWarning($"Type {objToAdd.GetType()} not ILateUpdatable");
                    }
                    break;
            }
        }

        public void RemoveFromLoop(GameLoopType loopType, object obj)
        {
            if (obj == null)
            {
                return;
            }
            
            switch (loopType)
            {
                case GameLoopType.Awake:
                    if (obj is IAwakable awakable)
                    {
                        foreach (var list in _awakeDictionary.Values)
                        {
                            list.Remove(awakable);
                        }
                    }
                    break;
                case GameLoopType.Update:
                    if (obj is IUpdatable updatable)
                    {
                        _updateDictionary.Remove(updatable);
                    }
                    break;
                case GameLoopType.FixedUpdate:
                    if (obj is IFixedUpdatable fixedUpdatable)
                    {
                        _fixedUpdateDictionary.Remove(fixedUpdatable);
                    }
                    break;
                case GameLoopType.LateUpdate:
                    if (obj is ILateUpdatable lateUpdatable)
                    {
                        _lateUpdateDictionary.Remove(lateUpdatable);
                    }
                    break;
            }
        }

        public void Awake()
        {
            OnAfterInit?.Invoke();
        }

        public void Update()
        {
            if (!_enabled)
            {
                return;
            }
            
            foreach (var u in _updateDictionary.Values)
            {
                u.UpdateCustom();
            }
        }

        public void FixedUpdate()
        {
            if (!_enabled)
            {
                return;
            }
            
            foreach (var f in _fixedUpdateDictionary.Values)
            {
                f.FixedUpdateCustom();
            }
        }

        public void LateUpdate()
        {
            if (!_enabled)
            {
                return;
            }
            
            foreach (var l in _lateUpdateDictionary.Values)
            {
                l.LateUpdateCustom();
            }
        }

        public void EnableUpdate(bool enable)
        {
            _enabled = enable;
        }
    }
}