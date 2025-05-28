using System;
using System.Collections.Generic;
using Zenject;

namespace Core
{
    public interface IGameLoopRunner
    {
        void AddToGameLoop(GameLoopType loopType, object objToAdd);
        void RemoveFromLoop(GameLoopType loopType, object obj);
        void EnableUpdate(bool enable);
    }
    
    public class GameLoopRunner : IGameLoopRunner, IInitializable, ITickable, IFixedTickable, ILateTickable, IDisposable
    {
        private readonly IGameLoop _innerGameLoop;
        private readonly DiContainer _container;
        private readonly ISceneLoader _sceneLoader;
        
        private readonly HashSet<IAwakable> _awakables = new();
        private readonly HashSet<IUpdatable> _updatables = new();
        private readonly HashSet<IFixedUpdatable> _fixedUpdatables = new();
        private readonly HashSet<ILateUpdatable> _lateUpdatables = new();
        
        private bool _initialized;

        public GameLoopRunner(
            IGameLoop innerGameLoop,
            DiContainer container,
            ISceneLoader sceneLoader)
        {
            _innerGameLoop = innerGameLoop;
            _container = container;
            _sceneLoader = sceneLoader;
        }

        public void Initialize()
        {
            SubscribeSceneLoad();
            RefreshSubscribers();
            _initialized = true;
        }
        
        private void SubscribeSceneLoad()
        {
            _sceneLoader.OnLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded()
        {
            RefreshSubscribers();
        }

        private void RefreshSubscribers()
        {
            AddNewSubscribers(_awakables, GameLoopType.Awake, a => a.AwakeCustom());
            AddNewSubscribers(_updatables, GameLoopType.Update);
            AddNewSubscribers(_fixedUpdatables, GameLoopType.FixedUpdate);
            AddNewSubscribers(_lateUpdatables, GameLoopType.LateUpdate);
            RemoveStaleSubscribers(_awakables, GameLoopType.Awake);
            RemoveStaleSubscribers(_updatables, GameLoopType.Update);
            RemoveStaleSubscribers(_fixedUpdatables, GameLoopType.FixedUpdate);
            RemoveStaleSubscribers(_lateUpdatables, GameLoopType.LateUpdate);
        }
        
        private void AddNewSubscribers<T>(HashSet<T> knownSet, GameLoopType loopType, Action<T> onAdd = null)
        {
            foreach (T obj in _container.ResolveAll<T>())
            {
                if (knownSet.Add(obj))
                {
                    _innerGameLoop.AddToGameLoop(loopType, obj);
                    onAdd?.Invoke(obj);
                }
            }
        }

        private void RemoveStaleSubscribers<T>(HashSet<T> knownSet, GameLoopType loopType)
        {
            var all = new HashSet<T>(_container.ResolveAll<T>());
            
            foreach (T obj in knownSet)
            {
                if (!all.Contains(obj))
                {
                    _innerGameLoop.RemoveFromLoop(loopType, obj);
                }
            }
            knownSet.IntersectWith(all);
        }

        public void Tick()
        {
            if (!_initialized)
            {
                return;
            }
            
            _innerGameLoop.Update();
        }

        public void FixedTick()
        {
            if (!_initialized)
            {
                return;
            }
            _innerGameLoop.FixedUpdate();
        }

        public void LateTick()
        {
            if (!_initialized)
            {
                return;
            }
            _innerGameLoop.LateUpdate();
        }

        public void AddToGameLoop(GameLoopType loopType, object objToAdd)
        {
            _innerGameLoop.AddToGameLoop(loopType, objToAdd);
        }

        public void RemoveFromLoop(GameLoopType loopType, object obj)
        {
            _innerGameLoop.RemoveFromLoop(loopType, obj);
        }

        public void EnableUpdate(bool enable)
        {
            _innerGameLoop.EnableUpdate(enable);
        }

        public void Dispose()
        {
            _sceneLoader.OnLoad -= SubscribeSceneLoad;
        }
    }
}