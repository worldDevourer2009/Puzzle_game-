using UnityEngine;

namespace Extensions
{
    public static class CameraExtensions
    {
        public static Vector3 GetCamForwardDirection(this Camera camera, Vector3 dir)
        {
            var forward = camera.transform.forward;
            forward.y = 0;
            forward.Normalize();

            var right = camera.transform.right;
            right.y = 0;
            right.Normalize();

            var moveDir = forward * dir.z + right * dir.x;
            return moveDir;
        }
    }
}