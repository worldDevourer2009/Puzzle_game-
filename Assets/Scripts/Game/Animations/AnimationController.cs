using UnityEngine;
using ILogger = Core.ILogger;

namespace Game
{
    public interface IAnimation
    {
        void InitAnimation(IEntity entity);
        void PlayIdle();
        void PlayJump();
        void PlayMove();
        void PlayRun();
    }

    public class AnimationController : IAnimation
    {
        private readonly ILogger _logger;
        private Animator _animator;
        private IEntity _entity;
        private int _layerIndex;
        private Vector3 _moveDir;
        private Vector3 _runDir;

        private static readonly int MoveForwardHash = Animator.StringToHash("MoveForward");
        private static readonly int MoveBackwardHash = Animator.StringToHash("MoveBackward");
        private static readonly int MoveLeftHash = Animator.StringToHash("MoveLeft");
        private static readonly int MoveRightHash = Animator.StringToHash("MoveRight");

        private static readonly int RunForwardHash = Animator.StringToHash("RunForward");
        private static readonly int RunBackwardHash = Animator.StringToHash("RunBackward");
        private static readonly int RunLeftHash = Animator.StringToHash("RunLeft");
        private static readonly int RunRightHash = Animator.StringToHash("RunRight");

        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int JumpHash = Animator.StringToHash("Jump");

        public AnimationController(ILogger logger)
        {
            _logger = logger;
        }

        public void InitAnimation(IEntity entity)
        {
            _entity = entity;

            if (_entity == null)
            {
                _logger.LogWarning("Entity is null");
                return;
            }

            _animator = _entity.EntityGA.GetComponent<Animator>();

            if (_animator == null)
            {
                _logger.LogWarning("There is no animator on the GA");
                return;
            }

            _entity.OnMove += HandleMove;
            _entity.OnRun += HandleRun;
            _entity.OnJump += PlayJump;
            _entity.OnIdle += PlayIdle;

            PlayIdle();
        }

        private void HandleRun(Vector3 dir)
        {
            _runDir = dir;
            PlayRun();
        }

        private void HandleMove(Vector3 dir)
        {
            _moveDir = dir;
            PlayMove();
        }

        public void PlayIdle()
        {
            if (CheckTrack(IdleHash))
                return;

            _animator.SetTrigger(IdleHash);
        }

        public void PlayJump()
        {
            if (CheckTrack(JumpHash))
                return;

            _animator.SetTrigger(JumpHash);
        }

        public void PlayMove()
        {
            if (_moveDir == Vector3.zero)
            {
                PlayIdle();
                return;
            }

            PlayMovementAnimation(_moveDir, MoveForwardHash, MoveBackwardHash, MoveLeftHash, MoveRightHash);
        }

        public void PlayRun()
        {
            if (_runDir == Vector3.zero)
            {
                PlayIdle();
                return;
            }
            
            PlayMovementAnimation(_runDir, RunForwardHash, RunBackwardHash, RunLeftHash, RunRightHash);
        }

        private void PlayMovementAnimation(Vector3 direction, int forwardHash, int backwardHash, int leftHash, int rightHash)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                if (direction.x > 0)
                {
                    _animator.SetTrigger(rightHash);
                }
                else
                {
                    _animator.SetTrigger(leftHash);
                }
            }
            else
            {
                if (direction.z > 0)
                {
                    _animator.SetTrigger(forwardHash);
                }
                else
                {
                    _animator.SetTrigger(backwardHash);
                }
            }
        }

        private bool CheckTrack(int hash)
        {
            var state = _animator.GetCurrentAnimatorStateInfo(_layerIndex);

            if (state.shortNameHash == hash)
            {
                return true;
            }

            return false;
        }

        private void TrySetTrigger(int hash)
        {
            if (CheckTrack(hash))
                return;

            _animator.SetTrigger(hash);
        }
    }
}