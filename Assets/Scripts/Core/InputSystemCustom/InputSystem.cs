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
        Use,
        Click,
        Run,
        Pause
    }
    
    public interface IInput
    {
        event Action<Vector3, bool> OnMoveAction;
        event Action<Vector3> OnLookAction;
        event Action<Vector3> OnClickAction;
        event Action OnNoneAction;
        event Action OnJumpAction;
        event Action OnUseAction;
        event Action OnPauseClicked;
        void EnableInput(bool enable);
    }

    public class InputSystem : IInput, IUpdatable, IFixedUpdatable, IAwakable
    {
        private readonly ICameraManager _cameraManager;
        private readonly IInputSourceComposite _inputSource;
        private readonly ILookSource _lookSource;
        private readonly ILogger _logger;
        
        public event Action<Vector3, bool> OnMoveAction;
        public event Action<Vector3> OnClickAction;
        public event Action OnNoneAction;
        public event Action OnJumpAction;
        public event Action OnUseAction;
        public event Action OnPauseClicked;

        private bool _enabledInput;
        
        public void EnableInput(bool enable)
        {
            _enabledInput = enable;
        }

        public event Action<Vector3> OnLookAction;

        public InputSystem(ILookSource lookSource, IInputSourceComposite inputSource, ILogger logger, ICameraManager cameraManager)
        {
            _lookSource = lookSource;
            _inputSource = inputSource;
            _logger = logger;
            _cameraManager = cameraManager;
        }
        
        public void AwakeCustom()
        {
            _enabledInput = true;
        }

        public void UpdateCustom()
        {
            if (!_enabledInput)
            {
                return;
            }
            
            HandleLook();
            HandleJump();
            HandleUse();
            HandleClick();
            HandleTest();
            HandlePause();
        }

        public void FixedUpdateCustom()
        {
            if (!_enabledInput)
            {
                return;
            }
            
            HandleMove();
        }

        private void HandlePause()
        {
            var isPauseClicked = _inputSource.Paused();

            if (!isPauseClicked)
            {
                return;
            }
            
            OnPauseClicked?.Invoke();
        }

        private async void HandleTest()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                await _cameraManager.SetActiveCamera(CustomCameraType.PlayerCamera);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                await _cameraManager.SetActiveCamera(CustomCameraType.UiCamera);
            }
        }

        private void HandleLook()
        {
            var lookDir = _lookSource.GetLookDelta();

            if (lookDir == Vector2.zero)
            {
                return;
            }
            
            OnLookAction?.Invoke(lookDir);
        }

        private void HandleClick()
        {
            var isClicked = _inputSource.Clicked(out var pos);

            if (!isClicked)
            {
                return;
            }
            
            OnClickAction?.Invoke(pos);
        }

        private void HandleMove()
        {
            var moveDir = _inputSource.GetMoveDirection();
            var isRunning = _inputSource.IsRunning();

            if (moveDir == Vector3.zero)
            {
                OnNoneAction?.Invoke();
                return;
            }

            OnMoveAction?.Invoke(moveDir, isRunning);
        }

        private void HandleJump()
        {
            var isJumping = _inputSource.Jumped();

            if (!isJumping)
            {
                return;
            }
            
            OnJumpAction?.Invoke();
        }
        
        private void HandleUse()
        {
            var isUsing = _inputSource.Pressed();

            if (!isUsing)
            {
                return;
            }
            
            OnUseAction?.Invoke();
        }
    }
}