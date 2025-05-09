using UnityEngine;

namespace Game
{
    public sealed class CameraComponent : ICamera
    {
        private float _xRotation;

        public CameraComponent()
        {
            _xRotation = 0f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void MoveCamera(GameObject camera, Vector3 direction, float clamp = 90)
        {
            _xRotation -= direction.y;
            _xRotation = Mathf.Clamp(_xRotation, -clamp, clamp);
            camera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }
    }
}