using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    [RequireComponent(typeof(Camera))]
    public sealed class UICamera : MonoBehaviour, ICamera
    {
        public CustomCameraType CameraType => CustomCameraType.UiCamera;
        public Camera Camera => _camera;
        public AudioListener AudioListener => _audioListener;

        private AudioListener _audioListener;
        private Camera _camera;

        [Inject]
        public void Construct()
        {
            _camera = GetComponent<Camera>();
            _audioListener = GetComponent<AudioListener>();
        }
    }
}