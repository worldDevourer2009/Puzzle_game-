using UnityEngine;

namespace Game
{
    public sealed class RotationLogic : IRotatable
    {
        public RotationLogic()
        {
        }

        public void Rotate(GameObject obj, Vector3 dir, RotationAxis axis)
        {
            if ((axis & RotationAxis.X) != 0)
            {
                obj.transform.Rotate(Vector3.right * dir.y);
            }
        
            if ((axis & RotationAxis.Y) != 0)
            {
                obj.transform.Rotate(Vector3.up * dir.x, Space.World);
            }
            
            if ((axis & RotationAxis.Z) != 0)
            {
                obj.transform.Rotate(Vector3.forward * dir.z);
            }
        }
    }
}