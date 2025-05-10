using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Cam : MonoBehaviour, IAwakable, IUpdatable
    {
        [Header("Interactions")]
        [SerializeField] private float _interactonDistance = 5f;
        [SerializeField] private LayerMask _interactionMask;
        
        private ICamera _camera;
        private IGameLoop _gameLoop;
        private IInput _input;
        private IRaycaster _raycaster;

        private Camera _mainCamera;

        [Inject]
        public void Construct(IGameLoop gameLoop, ICamera cam, IInput input, IRaycaster raycaster)
        {
            _camera = cam;
            _input = input;
            _gameLoop = gameLoop;
            _raycaster = raycaster;

            if (_gameLoop != null)
            {
                _gameLoop.AddToGameLoop(GameLoopType.Awake, this);
                _gameLoop.AddToGameLoop(GameLoopType.Update, this);
            }
        }
        
        public void AwakeCustom()
        {
            _input.OnLookAction += HandleCameraMovement;
            _mainCamera = Camera.main;
        }

        public void UpdateCustom()
        {
            if (_mainCamera == null)
            {
                Debug.Log("Main cam is null");
                return;
            }
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