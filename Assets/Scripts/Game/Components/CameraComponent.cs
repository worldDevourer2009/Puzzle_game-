using Core;
using UnityEngine;

namespace Game
{
    public sealed class CameraComponent : ICamera
    {
        private readonly InputConfig _inputConfig;
        private readonly float _cameraSensitivity;
        private float _xRotation;

        public CameraComponent(InputConfig inputConfig)
        {
            _xRotation = 0f;
            _inputConfig = inputConfig;

            if (_inputConfig != null)
            {
                _cameraSensitivity = _inputConfig.GetSensitivity();
            }
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void MoveCamera(GameObject camera, Vector3 direction, float clamp = 90)
        {
            var verticalInput = Mathf.Lerp(0, direction.y, _cameraSensitivity * Time.deltaTime);
            _xRotation -= verticalInput;
            _xRotation = Mathf.Clamp(_xRotation, -clamp, clamp);
            camera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }
    }
}