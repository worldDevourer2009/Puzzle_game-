using System;
using Core;
using UnityEngine;

namespace Game
{
    public interface IPlayerController
    {
        void InitInputSubs();
        void UnsubInput();
    }

    public sealed class PlayerController : IPlayerController, IAwakable, IDisposable
    {
        private readonly IGameLoop _gameLoop;
        private readonly IPlayerFacade _playerFacade;
        private readonly IPlayerInputHandler _playerInputHandler;
        private readonly Cam _cam;
        private readonly PlayerDefaultStatsConfig _defaultStatsConfig;

        private Action<Vector3, bool> _moveHandler;
        private Action<Vector3> _lookHandler;
        private Action _useHandler;
        private Action _jumpHandler;
        private Action _idleHandler;

        public PlayerController(IGameLoop gameLoop, IPlayerFacade playerFacade, IPlayerInputHandler playerInputHandler,
            Cam cam, PlayerDefaultStatsConfig defaultStatsConfig)
        {
            _gameLoop = gameLoop;
            _playerFacade = playerFacade;
            _playerInputHandler = playerInputHandler;
            _cam = cam;
            _defaultStatsConfig = defaultStatsConfig;

            if (_gameLoop != null)
            {
                _gameLoop.AddToGameLoop(GameLoopType.Awake, this);
            }
        }

        public void AwakeCustom()
        {
            _playerFacade.Initialize(_cam, _defaultStatsConfig.playerStats);
            InitInputSubs();
        }

        public void InitInputSubs()
        {
            _moveHandler = (dir, run) => _playerFacade.Move(dir, run);
            _lookHandler = (dir) => _playerFacade.Look(dir);
            _useHandler = () => _playerFacade.Use();
            _idleHandler = () => _playerFacade.Idle();
            _jumpHandler = () => _playerFacade.Jump();
            
            _playerInputHandler.OnMoveAction += _moveHandler;
            _playerInputHandler.OnJumpAction += _jumpHandler;
            _playerInputHandler.OnNoneAction += _idleHandler;
            _playerInputHandler.OnLookAction += _lookHandler;
            _playerInputHandler.OnUseAction += _useHandler;
        }

        public void UnsubInput()
        {
            _playerInputHandler.OnMoveAction -= _moveHandler;
            _playerInputHandler.OnJumpAction -= _jumpHandler;
            _playerInputHandler.OnNoneAction -= _idleHandler;
            _playerInputHandler.OnLookAction -= _lookHandler;
            _playerInputHandler.OnUseAction -= _useHandler;
        }

        public void Dispose()
        {
            _gameLoop.RemoveFromLoop(GameLoopType.Awake, this);
            UnsubInput();
        }
    }
}