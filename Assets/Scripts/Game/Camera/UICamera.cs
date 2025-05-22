using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    [RequireComponent(typeof(Camera))]
    public sealed class UICamera : MonoBehaviour, ICamera
    {
        public CustomCameraType CameraType => CustomCameraType.UiCamera;
        public Camera Camera
        {
            get
            {
                if (_camera == null || this == null)
                {
                    return null;
                }
                
                return _camera;
            }
        }
        
        private Camera _camera;

        [Inject]
        public void Construct()
        {
            _camera = GetComponent<Camera>();
        }
    }
}