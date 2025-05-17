using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;

namespace Core
{
    public enum CustomCameraType
    {
        PlayerCamera,
        UiCamera,
        NotInGameCamera
    }

    public interface ICameraManager
    {
        UniTask<ICamera> CreateCamera(CustomCameraType type, Transform parent = null);
        UniTask SetActiveCamera(CustomCameraType type, Transform parent = null);
        Camera GetActiveCamera();
        Camera GetMainCamera();
    }

    public class CameraManager : ICameraManager
    {
        private const string MainCameraTag = "MainCamera";
        private const string DisabledCamera = "DisabledCamera";

        private readonly IFactorySystem _factorySystem;
        private readonly ILogger _logger;
        private readonly Dictionary<CustomCameraType, HashSet<ICamera>> _cameras;

        public CameraManager(IFactorySystem factorySystem, ILogger logger)
        {
            _factorySystem = factorySystem;
            _logger = logger;
            _cameras = new Dictionary<CustomCameraType, HashSet<ICamera>>();
        }

        public async UniTask<ICamera> CreateCamera(CustomCameraType type, Transform parent = null)
        {
            var parsedType = type.ToString();

            try
            {
                var cam = await _factorySystem.Create(parsedType);

                if (!cam.TryGetComponent(out ICamera cameraComp))
                {
                    return null;
                }

                if (_cameras.TryGetValue(type, out var hashSet))
                {
                    hashSet.Add(cameraComp);
                }
                else
                {
                    _cameras[type] = new HashSet<ICamera>();
                    var newHashSet = _cameras[type];
                    newHashSet.Add(cameraComp);
                }

                if (parent != null)
                {
                    cameraComp.Camera.gameObject.transform.SetParent(parent, worldPositionStays: false);
                    cameraComp.Camera.gameObject.transform.position = parent.position;
                }

                return cameraComp;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to spawn camera with exception {exception.Message}");
                return null;
            }
        }

        public async UniTask SetActiveCamera(CustomCameraType type, Transform parent = null)
        {
            if (!_cameras.TryGetValue(type, out var typedCameras))
            {
                var cam = await CreateCamera(type, parent);

                if (cam.Camera == null)
                {
                    _logger.LogWarning("Camera does not contain ICamera Component");
                    return;
                }

                SetCamera(type);
            }
            else
            {
                SetCamera(type);
            }
        }

        private void SetCamera(CustomCameraType customCameraType)
        {
            DisableAllCameras();

            if (_cameras.TryGetValue(customCameraType, out var camHashSet))
            {
                if (camHashSet == null)
                {
                    _logger.LogWarning($"Camera(s) of type {customCameraType} doesn't exist");
                    return;
                }

                var cam = camHashSet.FirstOrDefault(x => x != null);

                if (cam != null && cam.Camera != null)
                {
                    cam.Camera.enabled = true;
                    cam.Camera.tag = MainCameraTag;
                }
            }

            SetCursor(customCameraType);
        }

        private void SetCursor(CustomCameraType customCameraType)
        {
            if (customCameraType == CustomCameraType.PlayerCamera)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
        }

        private void DisableAllCameras()
        {
            foreach (var camerasHashSets in _cameras.Values)
            {
                if (camerasHashSets == null || camerasHashSets.Count <= 0)
                {
                    continue;
                }

                foreach (var camera in camerasHashSets)
                {
                    if (camera.Camera == null)
                    {
                        continue;
                    }

                    camera.Camera.enabled = false;
                    camera.Camera.tag = DisabledCamera;
                }
            }
        }

        public Camera GetActiveCamera()
        {
            return Camera.allCameras.FirstOrDefault(c => c.enabled);
        }

        public Camera GetMainCamera()
        {
            return Camera.allCameras.FirstOrDefault(c => c.CompareTag(MainCameraTag));
        }
    }
}