using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Player : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        
        private IInput _input;
        private IMoveable _moveable;
        private IJumpable _jumpable;
        private PlayerDefaultStatsConfig _defaultStatsConfig;

        private float _jumpForce;
        private float _speed;

        [Inject]
        public void Constrcut(PlayerDefaultStatsConfig config, IInput input, IMoveable moveable, IJumpable jumpable)
        {
            _moveable = moveable;
            _jumpable = jumpable;
            _input = input;
            _defaultStatsConfig = config;

            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Start()
        {
            _input.MoveAction += HandleMove;
            _input.JumpAction += HandleJump;

            _speed = _defaultStatsConfig.playerStats.Speed;
            _jumpForce = _defaultStatsConfig.playerStats.JumpForce;
        }

        private void HandleJump()
        {
            _jumpable.Jump(_rigidbody, _jumpForce);
        }

        private void HandleMove(Vector3 dir)
        {
            _moveable.Move(gameObject, _speed, dir);
        }
    }
}