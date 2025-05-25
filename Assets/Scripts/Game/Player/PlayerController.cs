using System;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public interface IPlayerController
    {
        void InitInputSubs();
        void UnsubInput();
    }

    public sealed class PlayerController : IPlayerController, IDisposable
    {
        private readonly ILevelManager _levelManager;
        private readonly IPlayerInputHandler _playerInputHandler;

        private IPlayerFacade _playerFacade;

        private Action<Vector3, bool> _moveHandler;
        private Action<Vector3> _lookHandler;
        private Action _useHandler;
        private Action _jumpHandler;
        private Action _idleHandler;

        public PlayerController(ILevelManager levelManager, IPlayerInputHandler playerInputHandler)
        {
            _playerInputHandler = playerInputHandler;
            _levelManager = levelManager;

            if (_levelManager == null)
            {
                return;
            }

            _levelManager.OnPlayerCreated += () => InitPlayerFacade()
                .Forget();
        }

        private UniTask InitPlayerFacade()
        {
            var facade = _levelManager.PlayerEntity;

            if (facade != null)
            {
                _playerFacade = facade;
            }

            _playerFacade.Initialize();

            InitInputSubs();

            return UniTask.CompletedTask;
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
            UnsubInput();
        }
    }
}