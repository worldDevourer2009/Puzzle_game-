using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    [RequireComponent(typeof(Camera))]
    public sealed class PlayerCam : MonoBehaviour, ICamera
    {
        public CustomCameraType CameraType => CustomCameraType.PlayerCamera;
        public Camera Camera => _camera == null ? this.gameObject.GetComponent<Camera>() : _camera;
        
        private ICameraController _cameraController;
        private IInput _input;
        private Camera _camera;

        [Inject]
        public void Construct(ICameraController cam, IInput input)
        {
            _cameraController = cam;
            _input = input;
            
            _camera = GetComponent<Camera>();
            _input.OnLookAction += HandleCameraMovement;
        }

        private void HandleCameraMovement(Vector3 dir)
        {
            _cameraController.MoveCamera(dir);
        }

        public Vector3 GetCamForwardDirection(Vector3 dir)
        {
            var forward = gameObject.transform.forward;
            forward.y = 0;
            forward.Normalize();

            var right = gameObject.transform.right;
            right.y = 0;
            right.Normalize();

            var moveDir = forward * dir.z + right * dir.x;
            return moveDir;
        }
    }
}