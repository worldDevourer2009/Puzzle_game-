using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using ZLinq;

namespace Core
{
    public enum CustomCameraType
    {
        PlayerCamera,
        UiCamera,
        NotInGameCamera,
        Default
    }

    public interface ICameraManager
    {
        UniTask<ICamera> CreateCamera(CustomCameraType type, Transform parent = null);
        UniTask SetActiveCamera(CustomCameraType type, Transform parent = null);
        Camera GetActiveCamera();
        CustomCameraType GetActiveCameraType();
        Camera GetMainCamera();
        Camera GetPlayerCamera();
        void UnloadCameras();
        Camera GetCameraByType(CustomCameraType type);
    }

    public class CameraManager : ICameraManager
    {
        private const string MainCameraTag = "MainCamera";
        private const string DisabledCamera = "DisabledCamera";

        private readonly IFactorySystem _factorySystem;
        private readonly ILogger _logger;
        private readonly Dictionary<CustomCameraType, HashSet<ICamera>> _cameras;
        private readonly Dictionary<CustomCameraType, Camera> _activeCameras;

        public CameraManager(IFactorySystem factorySystem, ILogger logger)
        {
            _factorySystem = factorySystem;
            _logger = logger;
            _cameras = new Dictionary<CustomCameraType, HashSet<ICamera>>();
            _activeCameras = new Dictionary<CustomCameraType, Camera>();
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

                var aliveCams = camHashSet.AsValueEnumerable().Where(c => c != null && c.Camera != null);
                var cam = aliveCams.FirstOrDefault();

                if (cam != null && cam.Camera != null)
                {
                    cam.Camera.enabled = true;
                    cam.Camera.tag = MainCameraTag;
                    
                    _activeCameras[customCameraType] = cam.Camera;
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
                if (camerasHashSets == null || camerasHashSets.Count == 0)
                    continue;
                
                var toRemove = new List<ICamera>();

                foreach (var cameraComp in camerasHashSets)
                {
                    if (cameraComp == null || cameraComp.Camera == null)
                    {
                        toRemove.Add(cameraComp);
                        continue;
                    }

                    cameraComp.Camera.enabled = false;
                    cameraComp.Camera.tag = DisabledCamera;
                }
                
                foreach (var dead in toRemove)
                {
                    camerasHashSets.Remove(dead);
                }
            }
        }

        public Camera GetActiveCamera()
        {
            return _activeCameras.Values.AsValueEnumerable().FirstOrDefault(c => c.enabled);
        }

        public CustomCameraType GetActiveCameraType()
        {
            foreach (var pair in _activeCameras.AsValueEnumerable().Where(pair => pair.Value != null && pair.Value.enabled))
            {
                return pair.Key;
            }

            return CustomCameraType.Default;
        }

        public Camera GetMainCamera()
        {
            return _activeCameras.Values.AsValueEnumerable().FirstOrDefault(c => c.CompareTag(MainCameraTag));
        }

        public Camera GetPlayerCamera()
        {
            return _activeCameras.GetValueOrDefault(CustomCameraType.PlayerCamera);
        }

        public void UnloadCameras()
        {
            _cameras.Clear();
            _activeCameras.Clear();
        }

        public Camera GetCameraByType(CustomCameraType type)
        {
            return _activeCameras.GetValueOrDefault(type);
        }
    }
}