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

        void Initialize(PlayerFacade entity, Rigidbody rb);
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
        private readonly IRaycaster _raycaster;

        private readonly IPlayerDataHolder _playerDataHolder;
        private readonly IMoveable _moveable;
        private readonly IJumpable _jumpable;
        private readonly IRotatable _rotatable;
        private readonly IGroundable _groundable;
        private readonly IStepable _stepable;

        private PlayerFacade _entity;
        private GameObject _gameObject;
        private Rigidbody _rb;
        private Camera _playerCam;
        private bool _isGrounded;

        private MoveCallBack _moveCallBack;
        private AlwaysTrueFilter _alwaysTrueFilter;
        private RaycastParams _moveParams;

        public PlayerCore(Core.ILogger logger,
            IPlayerDataHolder dataHolder,
            IMoveable moveable,
            IJumpable jumpable,
            IRotatable rotatable,
            IGroundable groundable,
            IStepable stepable,
            ICameraManager cameraManager,
            IRaycaster raycaster)
        {
            _logger = logger;
            _playerDataHolder = dataHolder;
            _moveable = moveable;
            _jumpable = jumpable;
            _rotatable = rotatable;
            _groundable = groundable;
            _stepable = stepable;
            _cameraManager = cameraManager;
            _raycaster = raycaster;
        }

        public void Initialize(PlayerFacade entity, Rigidbody rb)
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
        }

        public void Move(Vector3 direction, bool isRunning = false)
        {
            if (_rb == null)
            {
                return;
            }

            _isGrounded = _groundable.IsGrounded(_entity, _playerDataHolder.PlayerGroundableParams.Value);

            if (CheckMoveDir(direction, out var moveDir))
            {
                return;
            }
            
            if (!CanMove(moveDir))
            {
                Step(moveDir);
                return;
            }
            
            _moveable.Move(_rb, !isRunning || !_isGrounded ? _playerDataHolder.PlayerSpeed.Value : _playerDataHolder.PlayerRunSpeed.Value, moveDir);
            FixZRotation();
            OnMove?.Invoke(direction, isRunning);
        }

        private void Step(Vector3 moveDir)
        {
            _stepable.Step(_rb, moveDir, _entity.BottomFoot, _entity.TopFoot, 0.3f);
        }

        private bool CanMove(Vector3 direction)
        {
            if (_entity.EntityGA == null)
            {
                return false;
            }
            
            _moveParams = new RaycastParams()
            {
                MaxDistance = 0.9f,
                Origin = _entity.BottomFoot.position,
                Direction = direction,
                LayerMask = _playerDataHolder.PlayerMoveInteractionLayerMask.Value
            };

            _moveCallBack = new MoveCallBack()
            {
                CanMove = true
            };
            
            _alwaysTrueFilter = new AlwaysTrueFilter();
            
            _raycaster.Raycast(ref _moveParams, ref _alwaysTrueFilter, ref _moveCallBack);
            
            return _moveCallBack.CanMove;
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

            _isGrounded = _groundable.IsGrounded(_entity, _playerDataHolder.PlayerGroundableParams.Value);

            if (!_isGrounded)
            {
                return;
            }

            _jumpable.Jump(_rb, _playerDataHolder.PlayerJumpForce.Value);
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

        private void FixZRotation()
        {
            if (_gameObject != null)
            {
                var euler = _gameObject.transform.eulerAngles;
                euler.x = 0f;
                euler.z = 0f;
                _gameObject.transform.rotation = Quaternion.Euler(euler);
            }
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