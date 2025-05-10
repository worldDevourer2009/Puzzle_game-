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
        
        private static readonly int MoveHash = Animator.StringToHash("Move");
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

            _entity.OnMove += PlayMove;
            _entity.OnJump += PlayJump;
            _entity.OnIdle += PlayIdle;
            
            PlayIdle();
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
            if (CheckTrack(MoveHash)) 
                return;
            
            _animator.SetTrigger(MoveHash);
        }

        public void PlayRun()
        {
            //none
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
    }
}