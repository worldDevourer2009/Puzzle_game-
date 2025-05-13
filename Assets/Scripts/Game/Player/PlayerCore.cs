using System;
using UnityEngine;

namespace Game
{
    public interface IPlayerCore
    {
        event Action<Vector3, bool> OnMove;
        event Action OnJump;
        event Action OnUse;
        event Action OnIdle;

        void Initialize(IEntity entity, Rigidbody rb, Cam cam, float speed, float runSpeed, float jumpForce);
        void Move(Vector3 direction, bool isRunning = false);
        void Rotate(Vector3 direction);
        void Jump();
        void Use();
        void Idle();
        IEntity GetPlayer();
    }

    public sealed class PlayerCore : IPlayerCore
    {
        public event Action<Vector3, bool> OnMove;
        public event Action OnJump;
        public event Action OnUse;
        public event Action OnIdle;

        private readonly Core.ILogger _logger;
        private readonly IMoveable _moveable;
        private readonly IJumpable _jumpable;
        private readonly IRotatable _rotatable;

        private IEntity _entity;
        private GameObject _gameObject;
        private Rigidbody _rb;
        private Cam _cam;
        
        private float _speed;
        private float _runSpeed;
        private float _jumpForce;

        public PlayerCore(Core.ILogger logger, IMoveable moveable, IJumpable jumpable, IRotatable rotatable)
        {
            _logger = logger;
            _moveable = moveable;
            _jumpable = jumpable;
            _rotatable = rotatable;
        }

        public void Initialize(IEntity entity, Rigidbody rb, Cam cam, float speed, float runSpeed, float jumpForce)
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
            
            if (cam == null)
            {
                _logger.LogError("Cam is null");
            }
            else
            {
                _cam = cam;
            }
            
            _speed = speed;
            _runSpeed = runSpeed;
            _jumpForce = jumpForce;
        }

        public void Move(Vector3 direction, bool isRunning = false)
        {
            if (_rb == null)
            {
                return;
            }
            
            var moveDir = _cam.GetCamForwardDirection(direction);
            _moveable.Move(_rb.gameObject, !isRunning ? _speed : _runSpeed, moveDir);
            OnMove?.Invoke(direction, isRunning);
        }

        public void Jump()
        {
            if (_rb == null)
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
            if (_gameObject == null)
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
    }
}