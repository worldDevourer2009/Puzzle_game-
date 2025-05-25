using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class LoadCamera : MonoBehaviour, ICamera
    {
        public CustomCameraType CameraType => CustomCameraType.LoadCamera;
        public Camera Camera => _camera;

        private Camera _camera;
        
        [Inject]
        public void Construct()
        {
            _camera = GetComponent<Camera>();
        }
    }
}