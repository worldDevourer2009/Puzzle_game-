using System;
using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    public interface IEntity
    {
        GameObject EntityGA { get; }
        event Action OnMove;
        event Action OnJump;
        event Action OnUse;
        event Action OnIdle;
    }
    
    public class Player : MonoBehaviour, IEntity
    {
        public GameObject EntityGA => this.gameObject;
        public event Action OnMove;
        public event Action OnJump;
        public event Action OnUse;
        public event Action OnIdle;

        private Rigidbody _rigidbody;

        private IInput _input;
        private IAnimation _animation;
        private Cam _cam;
        private IMoveable _moveable;
        private IJumpable _jumpable;
        private IRotatable _rotatable;
        private PlayerDefaultStatsConfig _defaultStatsConfig;

        private float _jumpForce;
        private float _speed;

        [Inject]
        public void Construct(PlayerDefaultStatsConfig config,
            IInput input, Cam cam,
            IMoveable moveable, IJumpable jumpable, IRotatable rotatable, IAnimation animation)
        {
            _defaultStatsConfig = config;
            _input = input;
            _cam = cam;
            _animation = animation;

            _moveable = moveable;
            _jumpable = jumpable;
            _rotatable = rotatable;

            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Start()
        {
            _input.OnMoveAction += HandleOnMove;
            _input.OnJumpAction += HandleOnJump;
            _input.OnLookAction += HandleLook;
            _input.OnUseAction += HandleUse;
            _input.OnNoneAction += HandleIdle;

            _speed = _defaultStatsConfig.playerStats.Speed;
            _jumpForce = _defaultStatsConfig.playerStats.JumpForce;
            _animation.InitAnimation(this);
        }

        private void HandleIdle()
        {
            OnIdle?.Invoke();
        }

        private void HandleUse()
        {
            //throw new NotImplementedException();
        }

        private void HandleLook(Vector3 dir)
        {
            _rotatable.Rotate(gameObject, dir, RotationAxis.Y);
        }

        private void HandleOnJump()
        {
            OnJump?.Invoke();
            _jumpable.Jump(_rigidbody, _jumpForce);
        }

        private void HandleOnMove(Vector3 dir)
        {
            OnMove?.Invoke();
            var moveDir = _cam.GetCamForwardDirection(dir);
            _moveable.Move(gameObject, _speed, moveDir);
        }
    }
}