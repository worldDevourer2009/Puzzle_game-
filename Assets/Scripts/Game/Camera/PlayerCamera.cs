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
        public AudioListener AudioListener => _audioListener;
        
        private Camera _camera;
        private AudioListener _audioListener;

        [Inject]
        private void Construct()
        {
            _camera = GetComponent<Camera>();
            _audioListener = GetComponent<AudioListener>();
        }
    }
}