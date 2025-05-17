using System;
using System.Collections.Generic;
using Zenject;

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

    public class GameLoop : IGameLoop, ITickable, ILateTickable, IFixedTickable, IInitializable
    {
        public event Action OnAfterInit;
        
        private readonly ILogger _logger;

        private readonly Dictionary<IAwakable, object> _awakeDictionary;
        private readonly Dictionary<IUpdatable, object> _updateDictionary;
        private readonly Dictionary<IFixedUpdatable, object> _fixedUpdateDictionary;
        private readonly Dictionary<ILateUpdatable, object> _lateUpdateDictionary;

        public GameLoop(ILogger logger)
        {
            _logger = logger;
            _awakeDictionary = new Dictionary<IAwakable, object>();
            _updateDictionary = new Dictionary<IUpdatable, object>();
            _fixedUpdateDictionary = new Dictionary<IFixedUpdatable, object>();
            _lateUpdateDictionary = new Dictionary<ILateUpdatable, object>();
        }

        public void AddToGameLoop(GameLoopType loopType, object objToAdd)
        {
            if (objToAdd == null)
                return;

            var type = objToAdd.GetType();

            switch (loopType)
            {
                case GameLoopType.Awake:
                    if (objToAdd is not IAwakable awakable)
                    {
                        _logger.LogWarning($"Object of type {type} does not implement IAwakable");
                        return;
                    }

                    if (!_awakeDictionary.TryAdd(awakable, objToAdd))
                    {
                        _logger.LogWarning("Trying to add the same awake to awake");
                    }

                    break;
                case GameLoopType.Update:
                    if (objToAdd is not IUpdatable updatable)
                    {
                        _logger.LogWarning($"Object of type {type} does not implement IUpdatable");
                        return;
                    }

                    if (!_updateDictionary.TryAdd(updatable, objToAdd))
                    {
                        _logger.LogWarning("Trying to add the same awake to awake");
                    }

                    break;
                case GameLoopType.FixedUpdate:
                    if (objToAdd is not IFixedUpdatable fixedUpdatable)
                    {
                        _logger.LogWarning($"Object of type {type} does not implement {nameof(IFixedUpdatable)}");
                        return;
                    }

                    if (!_fixedUpdateDictionary.TryAdd(fixedUpdatable, objToAdd))
                    {
                        _logger.LogWarning("Trying to add the same fixed update to fixedUpdate");
                    }

                    break;
                case GameLoopType.LateUpdate:
                    if (objToAdd is not ILateUpdatable lateUpdatable)
                    {
                        _logger.LogWarning($"Object of type {type} does not implement {nameof(ILateUpdatable)}");
                        return;
                    }

                    if (!_lateUpdateDictionary.TryAdd(lateUpdatable, objToAdd))
                    {
                        _logger.LogWarning("Trying to add the same late update to late update");
                    }

                    break;
                case GameLoopType.None:
                default:
                    _logger.LogError($"Out of range exception in game loop for object of type {type}");
                    break;
            }
        }

        public void RemoveFromLoop(GameLoopType loopType, object objToRemove)
        {
            if (objToRemove == null)
                return;

            var type = objToRemove.GetType();

            switch (loopType)
            {
                case GameLoopType.Awake:
                    if (objToRemove is not IAwakable awakable)
                    {
                        _logger.LogWarning($"Object of type {type} does not implement IAwakable");
                        return;
                    }

                    _awakeDictionary.Remove(awakable);

                    break;
                case GameLoopType.Update:
                    if (objToRemove is not IUpdatable updatable)
                    {
                        _logger.LogWarning($"Object of type {type} does not implement IUpdatable");
                        return;
                    }

                    _updateDictionary.Remove(updatable);
                    break;
                case GameLoopType.FixedUpdate:
                    if (objToRemove is not IFixedUpdatable fixedUpdatable)
                    {
                        _logger.LogWarning($"Object of type {type} does not implement {nameof(IFixedUpdatable)}");
                        return;
                    }

                    _fixedUpdateDictionary.Remove(fixedUpdatable);
                    break;
                case GameLoopType.LateUpdate:
                    if (objToRemove is not ILateUpdatable lateUpdatable)
                    {
                        _logger.LogWarning($"Object of type {type} does not implement {nameof(ILateUpdatable)}");
                        return;
                    }

                    _lateUpdateDictionary.Remove(lateUpdatable);
                    break;
                case GameLoopType.None:
                default:
                    _logger.LogError($"Out of range exception in game loop for object of type {type}");
                    break;
            }
        }

        public void Awake()
        {
            foreach (var awakable in _awakeDictionary.Values)
            {
                if (awakable == null)
                    continue;

                if (awakable is not IAwakable awakenableObj)
                    continue;

                awakenableObj.AwakeCustom();
            }
            
            OnAfterInit?.Invoke();
        }

        public void Update()
        {
            foreach (var updatable in _updateDictionary.Values)
            {
                if (updatable == null)
                    continue;

                if (updatable is not IUpdatable updatableObj)
                    continue;

                updatableObj.UpdateCustom();
            }
        }

        public void FixedUpdate()
        {
            foreach (var fixedUpdatable in _fixedUpdateDictionary.Values)
            {
                if (fixedUpdatable == null)
                    continue;

                if (fixedUpdatable is not IFixedUpdatable fixedUpdatableObj)
                    continue;

                fixedUpdatableObj.FixedUpdateCustom();
            }
        }

        public void LateUpdate()
        {
            foreach (var lateUpdatable in _lateUpdateDictionary.Values)
            {
                if (lateUpdatable == null)
                    continue;

                if (lateUpdatable is not ILateUpdatable lateUpdatableObj)
                    continue;

                lateUpdatableObj.LateUpdateCustom();
            }
        }

        public void Tick() =>
            Update();

        public void LateTick() =>
            LateUpdate();

        public void FixedTick() =>
            FixedUpdate();

        public void Initialize() =>
            Awake();
    }
}