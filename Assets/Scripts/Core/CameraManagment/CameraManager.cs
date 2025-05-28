using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using ZLinq;
using Object = UnityEngine.Object;

namespace Core
{
    public enum CustomCameraType
    {
        PlayerCamera,
        UiCamera,
        NotInGameCamera,
        LoadCamera,
        Default
    }

    public interface ICameraManager
    {
        event Action OnCameraChanged;
        UniTask<ICamera> CreateCamera(CustomCameraType type, Transform parent = null);
        UniTask SetActiveCamera(CustomCameraType type, Transform parent = null);
        Camera GetActiveCamera();
        CustomCameraType GetActiveCameraType();
        Camera GetMainCamera();
        Camera GetPlayerCamera();
        void UnloadCameras();
        void DestroyAllCameras();
        Camera GetCameraByType(CustomCameraType type);
    }

    public class CameraManager : ICameraManager
    {
        private const string MainCameraTag = "MainCamera";
        private const string DisabledCamera = "DisabledCamera";

        public event Action OnCameraChanged;
        
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
                if (_cameras.TryGetValue(type, out var activeCam))
                {
                    var activeCamera = activeCam.AsValueEnumerable().FirstOrDefault();
                    SetCameraParent(parent, activeCamera);
                    return activeCamera;
                }

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

                SetCameraParent(parent, cameraComp);

                return cameraComp;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    $"Failed to spawn camera with exception {exception.Message}, {exception.Source}, {exception.InnerException}");
                return null;
            }
        }

        private static void SetCameraParent(Transform parent, ICamera cameraComp)
        {
            if (parent != null)
            {
                cameraComp.Camera.gameObject.transform.SetParent(parent, worldPositionStays: false);
                cameraComp.Camera.gameObject.transform.position = parent.position;
            }
        }

        public async UniTask SetActiveCamera(CustomCameraType type, Transform parent = null)
        {
            ICamera cameraComp = null;
            if (_cameras.TryGetValue(type, out var cams))
            {
                cameraComp = cams.AsValueEnumerable().FirstOrDefault(c => c != null && c.Camera != null);
            }

            if (cameraComp == null)
            {
                cameraComp = await CreateCamera(type, parent);
                if (cameraComp == null || cameraComp.Camera == null)
                {
                    return;
                }
            }
            else
            {
                if (parent != null)
                {
                    cameraComp.Camera.transform.SetParent(parent, worldPositionStays: false);
                    cameraComp.Camera.transform.position = parent.position;
                }
            }

            SetCamera(type);
        }

        private void SetCamera(CustomCameraType customCameraType)
        {
            foreach (var cams in _activeCameras
                         .AsValueEnumerable()
                         .ToList()
                         .AsValueEnumerable()
                         .Where(cams => cams.Value == null))
            {
                _activeCameras.Remove(cams.Key);
            }

            DisableAllCameras();

            if (_cameras.TryGetValue(customCameraType, out var camHashSet))
            {
                if (camHashSet == null)
                {
                    return;
                }

                var aliveCams = camHashSet.AsValueEnumerable().Where(c => c != null && c.Camera != null);
                var cam = aliveCams.FirstOrDefault();

                if (cam != null && cam.Camera != null)
                {
                    cam.Camera.enabled = true;
                    cam.AudioListener.enabled = true;
                    cam.Camera.tag = MainCameraTag;
                    
                    _activeCameras[customCameraType] = cam.Camera;
                }
            }

            OnCameraChanged?.Invoke();
        }
        
        private void DisableAllCameras()
        {
            var toRemove = new List<ICamera>();
            
            foreach (var camerasHashSets in _cameras.Values)
            {
                if (camerasHashSets == null || camerasHashSets.Count == 0)
                    continue;

                foreach (var cameraComp in camerasHashSets)
                {
                    if (cameraComp == null || cameraComp.Camera == null)
                    {
                        toRemove.Add(cameraComp);
                        continue;
                    }

                    cameraComp.Camera.enabled = false;
                    cameraComp.Camera.tag = DisabledCamera;
                    cameraComp.AudioListener.enabled = false;
                }

                foreach (var dead in toRemove)
                {
                    camerasHashSets.Remove(dead);
                }
            }
        }

        public Camera GetActiveCamera()
        {
            return _activeCameras.Values.AsValueEnumerable().FirstOrDefault(c => c != null && c.enabled);
        }

        public CustomCameraType GetActiveCameraType()
        {
            foreach (var pair in _activeCameras.AsValueEnumerable()
                         .Where(pair => pair.Value != null && pair.Value.enabled))
            {
                return pair.Key;
            }

            return CustomCameraType.Default;
        }

        public Camera GetMainCamera()
        {
            var cam = _activeCameras.Values.AsValueEnumerable()
                .FirstOrDefault(c => c != null && c.CompareTag(MainCameraTag));

            if (cam != null)
            {
                return cam;
            }

            cam = Camera.main;

            if (cam == null)
            {
                var go = GameObject.FindGameObjectWithTag(MainCameraTag);

                if (go != null)
                {
                    cam = go.GetComponent<Camera>();
                }
            }

            if (cam != null)
            {
                _activeCameras[CustomCameraType.Default] = cam;
            }

            return cam;
        }

        public void DestroyAllCameras()
        {
            foreach (var cameraSet in _cameras.Values)
            {
                foreach (var cameraComp in cameraSet)
                {
                    if (cameraComp?.Camera != null)
                    {
                        Object.Destroy(cameraComp.Camera.gameObject);
                    }
                }
            }

            _cameras.Clear();
            _activeCameras.Clear();
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