using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class LoadCamera : MonoBehaviour, ICamera
    {
        public CustomCameraType CameraType => CustomCameraType.LoadCamera;
        public Camera Camera => _camera;
        public AudioListener AudioListener => _audioListener;

        private Camera _camera;
        private AudioListener _audioListener;
        
        [Inject]
        public void Construct()
        {
            _camera = GetComponent<Camera>();
            _audioListener = GetComponent<AudioListener>();
        }
    }
}