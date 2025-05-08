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
        event Action<Vector3> MoveAction;
        event Action JumpAction;
        event Action UseAction;
    }

    public class InputSystem : IInput, IUpdatable, IAwakable
    {
        private readonly IGameLoop _gameLoop;
        private readonly IInputSource _inputSource;
        
        public event Action<Vector3> MoveAction;
        public event Action JumpAction;
        public event Action UseAction;

        public InputSystem(IGameLoop gameLoop, IInputSource inputSource)
        {
            _gameLoop = gameLoop;
            _inputSource = inputSource;
            
            if (_gameLoop != null)
            {
                _gameLoop.AddToGameLoop(GameLoopType.Update, this);
                _gameLoop.AddToGameLoop(GameLoopType.Awake, this);
            }
        }
        
        public void Awake()
        {
        }

        public void Update()
        {
            HandleMove();
            HandleJump();
            HandleUse();
        }

        private void HandleMove()
        {
            var moveDir = _inputSource.GetMoveDirection();
            
            if (moveDir == Vector3.zero)
                return;
            
            Debug.Log($"Is moving with direction {moveDir}");
            MoveAction?.Invoke(moveDir);
        }

        private void HandleJump()
        {
            var isJumping = _inputSource.IsJumpPressed();

            if (!isJumping)
                return;
            
            Debug.Log("Is jumping");
            JumpAction?.Invoke();
        }
        
        private void HandleUse()
        {
            var isUsing = _inputSource.IsUsePressed();

            if (!isUsing)
                return;
            
            Debug.Log("Is using");
            UseAction?.Invoke();
        }
    }
}