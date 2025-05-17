using System;
using Core;
using Extensions;
using UnityEngine;

namespace Game
{
    public interface IPlayerCore
    {
        event Action<Vector3, bool> OnMove;
        event Action OnJump;
        event Action OnUse;
        event Action OnIdle;
        event Action OnFall;

        void Initialize(IEntity entity, Rigidbody rb, float speed, float runSpeed, float jumpForce,
            RaycastParams groundDistance);

        void Move(Vector3 direction, bool isRunning = false);
        void Rotate(Vector3 direction);
        void Jump();
        void Use();
        void Idle();
        IEntity GetPlayer();
    }

    public sealed class PlayerCore : IPlayerCore, IDisposable
    {
        public event Action<Vector3, bool> OnMove;
        public event Action OnJump;
        public event Action OnUse;
        public event Action OnIdle;
        public event Action OnFall;

        private readonly Core.ILogger _logger;

        private readonly ICameraManager _cameraManager;
        private readonly IMoveable _moveable;
        private readonly IJumpable _jumpable;
        private readonly IRotatable _rotatable;
        private readonly IGroundable _groundable;

        private IEntity _entity;
        private GameObject _gameObject;
        private Rigidbody _rb;
        private Camera _playerCam;

        private float _speed;
        private float _runSpeed;
        private bool _isGrounded;
        private float _jumpForce;
        private RaycastParams _groundParams;

        public PlayerCore(Core.ILogger logger,
            IMoveable moveable,
            IJumpable jumpable,
            IRotatable rotatable,
            IGroundable groundable,
            ICameraManager cameraManager)
        {
            _logger = logger;
            _moveable = moveable;
            _jumpable = jumpable;
            _rotatable = rotatable;
            _groundable = groundable;
            _cameraManager = cameraManager;
        }

        public void Initialize(IEntity entity, Rigidbody rb, float speed, float runSpeed, float jumpForce,
            RaycastParams groundDistance)
        {
            _entity = entity;

            if (_entity == null)
            {
                _logger.LogError("Entity is null");
            }
            else
            {
                _gameObject = _entity.EntityGA;
            }

            if (rb == null)
            {
                _logger.LogError("Rb is null");
            }
            else
            {
                _rb = rb;
            }

            var playerCam = _cameraManager.GetPlayerCamera();

            if (playerCam == null)
            {
                _logger.LogError("Cam is null");
            }
            else
            {
                _playerCam = playerCam;
            }

            _speed = speed;
            _runSpeed = runSpeed;
            _jumpForce = jumpForce;
            _groundParams = groundDistance;
        }

        public void Move(Vector3 direction, bool isRunning = false)
        {
            if (_rb == null)
            {
                return;
            }

            _isGrounded = _groundable.IsGrounded(_entity, _groundParams);

            if (CheckMoveDir(direction, out var moveDir))
            {
                return;
            }

            _moveable.Move(_rb.gameObject, !isRunning || !_isGrounded ? _speed : _runSpeed, moveDir);
            OnMove?.Invoke(direction, isRunning);
        }

        private bool CheckMoveDir(Vector3 direction, out Vector3 moveDir)
        {
            moveDir = Vector3.zero;

            if (_playerCam != null)
            {
                moveDir = _playerCam.GetCamForwardDirection(direction);
            }
            else
            {
                var cam = _cameraManager.GetPlayerCamera();

                if (cam == null)
                {
                    return true;
                }
                
                _playerCam = cam;
                moveDir = _playerCam.GetCamForwardDirection(direction);
            }

            return false;
        }

        public void Jump()
        {
            if (_rb == null)
            {
                return;
            }

            _isGrounded = _groundable.IsGrounded(_entity, _groundParams);

            if (!_isGrounded)
            {
                return;
            }

            _jumpable.Jump(_rb, _jumpForce);
            OnJump?.Invoke();
        }

        public void Use()
        {
            OnUse?.Invoke();
        }

        public void Rotate(Vector3 direction)
        {
            if (_gameObject == null || _cameraManager.GetActiveCameraType() != CustomCameraType.PlayerCamera)
            {
                return;
            }

            _rotatable.Rotate(_gameObject, direction, RotationAxis.Y);
        }

        public void Idle()
        {
            OnIdle?.Invoke();
        }

        public IEntity GetPlayer()
        {
            return _entity;
        }

        public void Dispose()
        {
        }
    }
}