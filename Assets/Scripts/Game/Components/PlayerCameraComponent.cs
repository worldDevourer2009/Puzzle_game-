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
        private readonly IPlayerInputHandler _playerInputHandler;
        private readonly ILogger _logger;
        
        private float _cameraSensitivity;
        private float _xRotation;
        private Camera _camera;

        public PlayerCameraControllerComponent(InputConfig inputConfig,
            ICameraManager cameraManager, IPlayerInputHandler playerInputHandler, ILogger logger)
        {
            _xRotation = 0f;
            _inputConfig = inputConfig;
            _cameraManager = cameraManager;
            _playerInputHandler = playerInputHandler;
            _logger = logger;

            _playerInputHandler.OnLookAction += MoveCameraHandler;
        }

        private void MoveCameraHandler(Vector3 dir)
        {
            MoveCamera(dir);
        }

        public UniTask InitCamera()
        {
            var cam = _cameraManager.GetPlayerCamera();
            
            if (cam != null)
            {
                _camera = cam;
                _cameraSensitivity = _inputConfig.GetSensitivity();
            }
            
            return UniTask.CompletedTask;
        }

        public async void MoveCamera(Vector3 direction, float clamp = 90)
        {
            if (_camera == null)
            {
                await InitCamera();
                return;
            }

            var verticalInput = Mathf.Lerp(0, direction.y, _cameraSensitivity * Time.deltaTime);
            _xRotation -= verticalInput;
            _xRotation = Mathf.Clamp(_xRotation, -clamp, clamp);
            _camera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }
    }
}