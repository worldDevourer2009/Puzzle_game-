using Core;
using UnityEngine;

namespace Game
{
    public class StepComponent : IStepable
    {
        private const float MaxStepSlopeAngle = 45;
        private const float StepCheckDistance = 0.3f;
        private const float StepMoveDistance = 0.3f;
        
        private readonly IRaycaster _raycaster;
        private readonly LayerMask _stepLayerMask = LayerMask.GetMask("Default");

        public StepComponent(IRaycaster raycaster)
        {
            _raycaster = raycaster;
        }

        public bool CanStep(Rigidbody rb, Vector3 moveDirection, Transform bottomRayOrigin, Transform topRayOrigin)
        {
            var dir = moveDirection.normalized;

            var bottomParams = new RaycastParams
            {
                Origin = bottomRayOrigin.position,
                Direction = dir,
                MaxDistance = StepCheckDistance,
                LayerMask = _stepLayerMask
            };

            var bottomCallback = new NormalCaptureCallback();
            var filter = new AlwaysTrueFilter();
            
            _raycaster.Raycast(ref bottomParams, ref filter, ref bottomCallback);

            if (!bottomCallback.HasHit)
            {
                return false;
            }

            if (Vector3.Angle(bottomCallback.HitNormal, Vector3.up) > MaxStepSlopeAngle)
            {
                return false;
            }

            var topParams = new RaycastParams
            {
                Origin = topRayOrigin.position,
                Direction = dir,
                MaxDistance = StepCheckDistance,
                LayerMask = _stepLayerMask
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
                return;
            }

            var dir = moveDirection.normalized;
            var stepOffset = dir * StepMoveDistance + Vector3.up * stepHeightOffset;
            rb.MovePosition(rb.position + stepOffset);
        }
    }
}