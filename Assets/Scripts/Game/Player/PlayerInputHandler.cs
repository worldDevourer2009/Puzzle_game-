using System;
using Core;
using Zenject;
using UnityEngine;

namespace Game
{
    public interface IPlayerInputHandler
    {
        event Action<Vector3, bool> OnMoveAction;
        event Action<Vector3> OnLookAction;
        event Action OnJumpAction;
        event Action OnUseAction;
        event Action OnNoneAction;
    }

    public sealed class PlayerInputHandler : IPlayerInputHandler, IInitializable, IDisposable
    {
        public event Action<Vector3, bool> OnMoveAction;
        public event Action<Vector3> OnLookAction;
        public event Action OnJumpAction;
        public event Action OnUseAction;
        public event Action OnNoneAction;

        private readonly IInput _input;

        public PlayerInputHandler(IInput input)
        {
            _input = input;
        }
        
        public void Initialize()
        {
            SubToInput();
        }

        private void SubToInput()
        {
            _input.OnMoveAction += HandleOnMove;
            _input.OnJumpAction += HandleOnJump;
            _input.OnLookAction += HandleLook;
            _input.OnUseAction += HandleUse;
            _input.OnNoneAction += HandleIdle;
        }

        private void HandleLook(Vector3 dir)
        {
            OnLookAction?.Invoke(dir);
        }

        private void HandleIdle()
        {
            OnNoneAction?.Invoke();
        }

        private void HandleUse()
        {
            OnUseAction?.Invoke();
        }

        private void HandleOnJump()
        {
            OnJumpAction?.Invoke();
        }

        private void HandleOnMove(Vector3 dir, bool isRunning)
        {
            OnMoveAction?.Invoke(dir, isRunning);
        }

        public void Dispose()
        {
            _input.OnMoveAction -= HandleOnMove;
            _input.OnJumpAction -= HandleOnJump;
            _input.OnLookAction -= HandleLook;
            _input.OnUseAction -= HandleUse;
            _input.OnNoneAction -= HandleIdle;
        }
    }
}