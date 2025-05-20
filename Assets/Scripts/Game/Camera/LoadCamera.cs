using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class LoadCamera : MonoBehaviour, ICamera
    {
        public CustomCameraType CameraType => CustomCameraType.LoadCamera;
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