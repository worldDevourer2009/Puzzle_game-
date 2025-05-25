using System;

namespace Core
{
    [Serializable]
    public struct PlayerStats
    {
        public float Health;
        public float Speed;
        public float JumpForce;
        public float RunSpeed;
        public float CrouchSpeed;
        public RaycastParams GroundRaycastParams;
        
        public float LookClamp;
        
        public float MaxStepSlopeAngle;
        public float StepCheckDistance;
        public float StepHeight;
        public float StepMoveDistance;
    }
}