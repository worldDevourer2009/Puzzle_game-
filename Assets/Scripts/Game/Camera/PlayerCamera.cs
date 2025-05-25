using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    [RequireComponent(typeof(Camera))]
    public sealed class PlayerCamera : MonoBehaviour, ICamera
    {
        public CustomCameraType CameraType => CustomCameraType.PlayerCamera;
        public Camera Camera => _camera == null ? this.gameObject.GetComponent<Camera>() : _camera;
        private Camera _camera;

        [Inject]
        private void Construct()
        {
            _camera = GetComponent<Camera>();
        }
    }
}