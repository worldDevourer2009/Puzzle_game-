using System;
using UnityEngine;

namespace Core
{
    public enum InputAction
    {
        MoveForward,
        MoveBackward,
        MoveLeft,
        MoveRight,
        Jump,
        Use
    }
    
    public interface IInput
    {
        event Action<Vector3> OnMoveAction;
        event Action<Vector3> OnLookAction;
        event Action OnJumpAction;
        event Action OnUseAction;
    }

    public class InputSystem : IInput, IUpdatable, IAwakable
    {
        private readonly IGameLoop _gameLoop;
        private readonly IInputSource _inputSource;
        private readonly ILookSource _lookSource;
        private readonly ILogger _logger;
        
        public event Action<Vector3> OnMoveAction;
        public event Action OnJumpAction;
        public event Action OnUseAction;
        public event Action<Vector3> OnLookAction;

        public InputSystem(IGameLoop gameLoop, ILookSource lookSource, IInputSource inputSource, ILogger logger)
        {
            _gameLoop = gameLoop;
            _lookSource = lookSource;
            _inputSource = inputSource;
            _logger = logger;
            
            if (_gameLoop != null)
            {
                _gameLoop.AddToGameLoop(GameLoopType.Update, this);
                _gameLoop.AddToGameLoop(GameLoopType.Awake, this);
            }
        }
        
        public void AwakeCustom()
        {
        }

        public void UpdateCustom()
        {
            HandleMove();
            HandleLook();
            HandleJump();
            HandleUse();
        }

        private void HandleLook()
        {
            var lookDir = _lookSource.GetLookDelta();

            if (lookDir == Vector2.zero)
                return;
            
            OnLookAction?.Invoke(lookDir);
        }

        private void HandleMove()
        {
            var moveDir = _inputSource.GetMoveDirection();
            
            if (moveDir == Vector3.zero)
                return;
            
            OnMoveAction?.Invoke(moveDir);
        }

        private void HandleJump()
        {
            var isJumping = _inputSource.IsJumpPressed();

            if (!isJumping)
                return;
            
            OnJumpAction?.Invoke();
        }
        
        private void HandleUse()
        {
            var isUsing = _inputSource.IsUsePressed();

            if (!isUsing)
                return;
            
            OnUseAction?.Invoke();
        }
    }
}