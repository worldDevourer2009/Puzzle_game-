using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ILogger = Core.ILogger;

namespace Game
{
    public sealed class PlayerCameraControllerComponent : ICameraController
    {
        private readonly InputConfig _inputConfig;
        private readonly ICameraManager _cameraManager;
        private readonly ILevelManager _levelManager;
        private readonly ILogger _logger;
        private readonly float _cameraSensitivity;
        private float _xRotation;
        private Camera _camera;

        public PlayerCameraControllerComponent(InputConfig inputConfig, 
            ICameraManager cameraManager, ILevelManager levelManager, ILogger logger)
        {
            _xRotation = 0f;
            _inputConfig = inputConfig;
            _cameraManager = cameraManager;
            _levelManager = levelManager;
            _logger = logger;

            if (_inputConfig != null)
            {
                _cameraSensitivity = _inputConfig.GetSensitivity();
            }

            _levelManager.OnPlayerCreated += () => InitCamera();
        }

        public UniTask InitCamera()
        {
            var cam =  _cameraManager.GetMainCamera();

            if (cam != null)
            {
                _camera = cam;
            }
            
            return UniTask.CompletedTask;
        }

        public void MoveCamera(Vector3 direction, float clamp = 90)
        {
            if (_camera == null)
            {
                _logger.LogWarning("Camera is null, can't move it");
                return;
            }
            
            Debug.Log("Moving camera");
            
            var verticalInput = Mathf.Lerp(0, direction.y, _cameraSensitivity * Time.deltaTime);
            _xRotation -= verticalInput;
            _xRotation = Mathf.Clamp(_xRotation, -clamp, clamp);
            _camera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }
    }
}