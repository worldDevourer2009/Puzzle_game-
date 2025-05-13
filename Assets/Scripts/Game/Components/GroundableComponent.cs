using Core;
using UnityEngine;
using ILogger = Core.ILogger;

namespace Game
{
    public class GroundableComponent : IGroundable
    {
        private readonly IRaycaster _raycaster;
        private readonly ILogger _logger;
        
        private GroundFilter _groundFilter;
        private GroundCallback _groundCallback;
        private RaycastParams _raycastParams;
        private bool _isGrounded;

        public GroundableComponent(IRaycaster raycaster, ILogger logger)
        {
            _raycaster = raycaster;
            _logger = logger;

            _groundFilter = new GroundFilter();
            _groundCallback = new GroundCallback();

            _groundCallback.OnGroundHit += SetGrounded;
        }

        private void SetGrounded()
        {
            _isGrounded = true;
        }

        public bool IsGrounded(IEntity entity, RaycastParams paramsRaycast)
        {
            if (entity.EntityGA == null)
            {
                _logger.LogWarning("Game object is null");
                return true;
            }

            _isGrounded = false;
            
            paramsRaycast.Direction = Vector3.down;
            paramsRaycast.Origin = entity.CenterBottomTransform.position;
            
            _raycaster.Raycast(ref paramsRaycast, ref _groundFilter, ref _groundCallback);
            
            return _isGrounded;
        }
    }
}