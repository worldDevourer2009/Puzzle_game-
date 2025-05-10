using System;
using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    public interface IEntity
    {
        GameObject EntityGA { get; }
        event Action<Vector3> OnMove;
        event Action OnJump;
        event Action OnUse;
        event Action<Vector3> OnRun;
        event Action OnIdle;
    }

    public class Player : MonoBehaviour, IEntity
    {
        public GameObject EntityGA => this.gameObject;
        public event Action<Vector3> OnMove;
        public event Action OnJump;
        public event Action OnUse;
        public event Action<Vector3> OnRun;
        public event Action OnIdle;

        private Rigidbody _rigidbody;

        private IInput _input;
        private IAnimation _animation;
        private Cam _cam;
        private PlayerDefaultStatsConfig _defaultStatsConfig;
        private IPlayerCore _core;

        private float _jumpForce;
        private float _speed;

        [Inject]
        public void Construct(PlayerDefaultStatsConfig config,
            IInput input, Cam cam, IPlayerCore core, IAnimation animatioHandler)
        {
            _defaultStatsConfig = config;
            _input = input;
            _cam = cam;
            _animation = animatioHandler;
            _core = core;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _speed = _defaultStatsConfig.playerStats.Speed;
            _jumpForce = _defaultStatsConfig.playerStats.JumpForce;

            _core.Initialize(_rigidbody, _cam, _speed, _jumpForce);

            _core.OnMove += (x, y) =>
            {
                if (!y)
                {
                    OnMove?.Invoke(x);
                }
                else
                {
                    OnRun?.Invoke(x);
                }
            };
            
            _core.OnJump += () => OnJump?.Invoke();
            _core.OnUse += () => OnUse?.Invoke();
            _core.OnIdle += () => OnIdle?.Invoke();
        }

        public void Start()
        {
            _input.OnMoveAction += HandleOnMove;
            _input.OnJumpAction += HandleOnJump;
            _input.OnLookAction += HandleLook;
            _input.OnUseAction += HandleUse;
            _input.OnNoneAction += HandleIdle;

            _animation.InitAnimation(this);
        }

        private void HandleOnMove(Vector3 dir, bool isRunning)
        {
            _core.Move(dir, isRunning);
        }

        private void HandleIdle()
        {
            _core.Idle();
        }

        private void HandleUse()
        {
            _core.Use();
        }

        private void HandleLook(Vector3 dir)
        {
            _core.Rotate(gameObject, dir);
        }

        private void HandleOnJump()
        {
            _core.Jump();
        }
    }
}