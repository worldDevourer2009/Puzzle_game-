using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Cam : MonoBehaviour
    {
        private ICamera _camera;
        private IInput _input;

        [Inject]
        public void Construct(ICamera cam, IInput input)
        {
            _camera = cam;
            _input = input;
        }

        public void Start()
        {
            _input.OnLookAction += HandleCameraMovement;
        }

        private void HandleCameraMovement(Vector3 dir)
        {
            _camera.MoveCamera(gameObject, dir);
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