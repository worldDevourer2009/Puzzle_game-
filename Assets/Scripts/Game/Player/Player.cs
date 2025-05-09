using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Player : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        private IInput _input;
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
            IMoveable moveable, IJumpable jumpable, IRotatable rotatable)
        {
            _defaultStatsConfig = config;
            _input = input;
            _cam = cam;

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

            _speed = _defaultStatsConfig.playerStats.Speed;
            _jumpForce = _defaultStatsConfig.playerStats.JumpForce;
        }

        private void HandleLook(Vector3 dir)
        {
            _rotatable.Rotate(gameObject, dir, RotationAxis.Y);
        }

        private void HandleOnJump()
        {
            _jumpable.Jump(_rigidbody, _jumpForce);
        }

        private void HandleOnMove(Vector3 dir)
        {
            var moveDir = _cam.GetCamForwardDirection(dir);
            _moveable.Move(gameObject, _speed, moveDir);
        }
    }
}