using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    [RequireComponent(typeof(Camera))]
    public sealed class UiCam : MonoBehaviour, ICamera
    {
        public CustomCameraType CameraType => CustomCameraType.UiCamera;
        public Camera Camera => _camera == null ? this.gameObject.GetComponent<Camera>() : _camera;
        private Camera _camera;

        [Inject]
        public void Construct()
        {
            _camera = GetComponent<Camera>();
        }
    }
}