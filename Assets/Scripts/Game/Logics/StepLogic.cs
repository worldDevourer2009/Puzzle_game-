using Core;
using UnityEngine;
using Logger = Core.Logger;

namespace Game
{
    public class StepLogic : IStepable
    {
        private readonly IPlayerDataHolder _playerDataHolder;
        private readonly IRaycaster _raycaster;

        public StepLogic(IRaycaster raycaster, IPlayerDataHolder playerDataHolder)
        {
            _raycaster = raycaster;
            _playerDataHolder = playerDataHolder;
        }

        public bool CanStep(Rigidbody rb, Vector3 moveDirection, Transform bottomRayOrigin, Transform topRayOrigin)
        {
            var dir = moveDirection.normalized;

            var bottomParams = new RaycastParams
            {
                Origin = bottomRayOrigin.position,
                Direction = dir,
                MaxDistance = _playerDataHolder.PlayerStepCheckDistance.Value,
                LayerMask = _playerDataHolder.PlayerStepInteractionLayerMask.Value
            };

            var bottomCallback = new NormalCaptureCallback();
            var filter = new AlwaysTrueFilter();
            
            _raycaster.Raycast(ref bottomParams, ref filter, ref bottomCallback);

            if (!bottomCallback.HasHit)
            {
                return false;
            }

            if (Vector3.Angle(bottomCallback.HitNormal, Vector3.up) > _playerDataHolder.PlayerMaxStepSlopeAngle.Value)
            {
                return false;
            }

            var topParams = new RaycastParams
            {
                Origin = topRayOrigin.position,
                Direction = dir,
                MaxDistance = _playerDataHolder.PlayerStepCheckDistance.Value,
                LayerMask = _playerDataHolder.PlayerStepInteractionLayerMask.Value
            };

            var topCallback = new HitDetectedCallback();
            _raycaster.Raycast(ref topParams, ref filter, ref topCallback);

            return topCallback.HasHit;
        }

        public void Step(Rigidbody rb, Vector3 moveDirection, Transform bottomRayOrigin, Transform topRayOrigin,
            float stepHeightOffset)
        {
            if (!CanStep(rb, moveDirection, bottomRayOrigin, topRayOrigin))
            {
                Logger.Instance.Log("Can't step");
                return;
            }
            
            Logger.Instance.Log("Stepping");

            var dir = moveDirection.normalized;
            var stepOffset = dir * _playerDataHolder.PlayerStepMoveDistance.Value + Vector3.up * stepHeightOffset;
            rb.MovePosition(rb.position + stepOffset);
        }
    }
}