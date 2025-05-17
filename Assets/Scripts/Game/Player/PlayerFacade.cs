using System;
using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class PlayerFacade : MonoBehaviour, IPlayerFacade
    {
        public Transform EyesTransform => _eyesTransform;
        public GameObject EntityGA => gameObject;
        public Transform RightHandTransform => _rightHandTransform;
        public Transform LeftHandTransform => _leftHandTransform;
        public Transform CenterBottomTransform => _centerBottomTransform;

        public event Action<Vector3, bool> OnMove;
        public event Action OnJump;
        public event Action OnUse;
        public event Action OnIdle;
        
        [SerializeField] private Transform _rightHandTransform;
        [SerializeField] private Transform _leftHandTransform;
        [SerializeField] private Transform _centerBottomTransform;
        [SerializeField] private Transform _eyesTransform;
        [SerializeField] private Rigidbody _rigidbody;
        
        private IPlayerCore _core;
        private IAnimation _animation;

        private Action<Vector3, bool> _moveHandler;
        private Action _useHandler;
        private Action _jumpHandler;
        private Action _idleHandler;

        [Inject]
        public void Construct(IPlayerCore core, IAnimation animation)
        {
            _core = core;
            _animation = animation;
            
            _moveHandler = (dir, run) => OnMove?.Invoke(dir, run);
            _useHandler = () => OnUse?.Invoke();
            _jumpHandler = () => OnJump?.Invoke();
            _idleHandler = () => OnIdle?.Invoke();
            
            _core.OnMove += _moveHandler;
            _core.OnJump += _jumpHandler;
            _core.OnUse += _useHandler;
            _core.OnIdle += _idleHandler;
            
            Debug.LogWarning("Injecting player facade");
        }

        public void Initialize(PlayerStats stats)
        {
            _core.Initialize(this, _rigidbody, stats.Speed, stats.RunSpeed, stats.JumpForce, stats.GroundRaycastParams);
            _animation.InitAnimation(this);
        }

        public void Move(Vector3 direction, bool run = false)
        {
            _core.Move(direction, run);
        }

        public void Jump()
        {
            _core.Jump();
        }

        public void Use()
        {
            _core.Use();
        }

        public void Idle()
        {
            _core.Idle();
        }

        public void Look(Vector3 lookDirection)
        {
            _core.Rotate(lookDirection);
        }

        private void OnDestroy()
        {
            _core.OnMove -= _moveHandler;
            _core.OnJump -= _jumpHandler;
            _core.OnUse -= _useHandler;
            _core.OnIdle -= _idleHandler;
        }
    }
}