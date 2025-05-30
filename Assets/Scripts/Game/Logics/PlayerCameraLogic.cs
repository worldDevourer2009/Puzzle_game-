using System;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public sealed class PlayerCameraLogic : IPlayerCameraLogic, IDisposable
    {
        private readonly IInputDataHolder _inputDataHolder;
        private readonly IPlayerDataHolder _playerDataHolder;
        private readonly ICameraManager _cameraManager;
        private readonly IPlayerInputHandler _playerInputHandler;
        
        private float _xRotation;
        private Camera _camera;

        public PlayerCameraLogic(IInputDataHolder inputDataHolder, IPlayerDataHolder playerDataHolder,
            ICameraManager cameraManager, IPlayerInputHandler playerInputHandler)
        {
            _xRotation = 0f;
            _inputDataHolder = inputDataHolder;
            _playerDataHolder = playerDataHolder;
            _cameraManager = cameraManager;
            _playerInputHandler = playerInputHandler;

            _playerInputHandler.OnLookAction += MoveCameraHandler;
        }

        private void MoveCameraHandler(Vector3 dir)
        {
            MoveCamera(dir).Forget();
        }

        public UniTask InitCamera()
        {
            var cam = _cameraManager.GetPlayerCamera();
            
            if (cam != null)
            {
                _camera = cam;
            }
            
            return UniTask.CompletedTask;
        }

        public async UniTaskVoid MoveCamera(Vector3 direction)
        {
            if (_camera == null)
            {
                await InitCamera();
                
                if (_camera == null)
                {
                    return;
                }
            }
            
            var clamp = _playerDataHolder.PlayerLookClamp.Value;
            var verticalInput = direction.y * _inputDataHolder.Sensitivity.Value * Time.deltaTime;
            _xRotation -= verticalInput;
            _xRotation = Mathf.Clamp(_xRotation, -clamp, clamp);
            _camera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }

        public void Dispose()
        {
            _playerInputHandler.OnLookAction -= MoveCameraHandler;
        }
    }
}