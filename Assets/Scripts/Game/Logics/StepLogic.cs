using Core;
using UnityEngine;

namespace Game
{
    public class StepLogic : IStepable
    {
        private const float DownDistThreshold = 0.05f;
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

            if (!TryCast(bottomRayOrigin.position, dir, _playerDataHolder.PlayerStepCheckDistance.Value,
                    out var frontHit))
            {
                return false;
            }

            if (TryCast(topRayOrigin.position, dir, _playerDataHolder.PlayerStepCheckDistance.Value, out _))
            {
                return false;
            }

            var downOrigin = bottomRayOrigin.position + dir * _playerDataHolder.PlayerStepCheckDistance.Value +
                             Vector3.up * _playerDataHolder.PlayerStepHeight.Value;
            
            var downDist = _playerDataHolder.PlayerStepHeight.Value + DownDistThreshold;
            
            if (!TryCast(downOrigin, Vector3.down, downDist, out var downHit))
            {
                return false;
            }

            if (Vector3.Angle(downHit.normal, Vector3.up) > _playerDataHolder.PlayerMaxStepSlopeAngle.Value)
            {
                return false;
            }

            return true;
        }

        public void Step(Rigidbody rb, Vector3 moveDirection, Transform bottomRayOrigin, Transform topRayOrigin,
            float stepHeightOffset)
        {
            if (!CanStep(rb, moveDirection, bottomRayOrigin, topRayOrigin))
            {
                return;
            }

            var dir = moveDirection.normalized;
            var stepOffset = dir * _playerDataHolder.PlayerStepMoveDistance.Value + Vector3.up * stepHeightOffset;
            rb.MovePosition(rb.position + stepOffset);
        }

        private bool TryCast(Vector3 origin, Vector3 dir, float dist, out RaycastHit hit)
        {
            hit = default;

            var prms = new RaycastParams
            {
                Origin = origin,
                Direction = dir,
                MaxDistance = dist,
                LayerMask = _playerDataHolder.PlayerStepInteractionLayerMask.Value
            };

            var callback = new HitDetectedCallback();
            var filter = new AlwaysTrueFilter();

            _raycaster.Raycast(ref prms, ref filter, ref callback);

            if (callback.HasHit)
            {
                hit = callback.Hit;
            }

            return callback.HasHit;
        }
    }
}