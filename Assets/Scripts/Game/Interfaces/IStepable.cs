using UnityEngine;

namespace Game
{
    public interface IStepable
    {
        void Step(Rigidbody rb, Vector3 moveDirection, Transform bottomRayOrigin, Transform topRayOrigin,
            float stepHeightOffset);
        bool CanStep(Rigidbody rb, Vector3 moveDirection, Transform bottomRayOrigin, Transform topRayOrigin);
    }
}