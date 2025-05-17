using System;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ILogger = Core.ILogger;

namespace Game
{
    public interface IPlayerInteractor
    {
        event Action<IInteractable> OnInteract; 
        event Action<IInteractable> OnPoint;
        void Interact();
        void Release();
    }

    public class PlayerInteractor : IPlayerInteractor, IAwakable, IUpdatable, IDisposable
    {
        public event Action<IInteractable> OnInteract;
        public event Action<IInteractable> OnPoint;

        private readonly IInput _input;
        private readonly IGameLoop _gameLoop;
        private readonly IRaycaster _raycaster;
        private readonly ILevelManager _levelManager;
        private readonly PlayerInteractionConfig _playerInteractionConfig;
        private readonly ICameraManager _cameraManager;
        private readonly ILogger _logger;
        private readonly IInteractorCore _interactorCore;

        private Camera _mainCamera;
        private float _interactonDistance;
        private LayerMask _interactionMask;
        
        private InteractableFilter _filter;
        private InteractableCallback _callback;

        public PlayerInteractor(IInteractorCore interactorCore, IInput input, IGameLoop gameLoop,
            IRaycaster raycaster, PlayerInteractionConfig playerInteractionConfig, ILogger logger, ILevelManager levelManager)
        {
            _interactorCore = interactorCore;
            _input = input;
            _gameLoop = gameLoop;
            _raycaster = raycaster;
            _playerInteractionConfig = playerInteractionConfig;
            _logger = logger;
            _levelManager = levelManager;

            _filter = new InteractableFilter();
            _callback = new InteractableCallback();

            if (_gameLoop != null)
            {
                _gameLoop.AddToGameLoop(GameLoopType.Awake, this);
                _gameLoop.AddToGameLoop(GameLoopType.Update, this);
            }
        }

        public void AwakeCustom()
        {
            _input.OnClickAction += HandleInput;
            _interactorCore.OnPoint += HandlePoint;
            _interactorCore.OnInteract += HandleInteract;
            _levelManager.OnPlayerCreated += InitPlayerCamera;
            
            _interactionMask = _playerInteractionConfig.PlayerInteraction.LayerMask;
            _interactonDistance = _playerInteractionConfig.PlayerInteraction.InteractionDistance;
        }

        private void InitPlayerCamera()
        {
            _mainCamera = Camera.main;
        }

        private void HandleInteract(IInteractable candidate)
        {
            OnInteract?.Invoke(candidate);
        }

        private void HandlePoint(IInteractable candid)
        {
            OnPoint?.Invoke(candid);
        }

        private void HandleInput(Vector3 obj)
        {
            Interact();
        }

        public void UpdateCustom()
        {
            CheckInteraction();
        }

        private void CheckInteraction()
        {
            if (_mainCamera == null || !_mainCamera.enabled)
            {
                return;
            }
            
            var ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            
            var raycastParams = new RaycastParams
            {
                Origin = ray.origin,
                Direction = ray.direction,
                MaxDistance = _interactonDistance,
                LayerMask = _interactionMask
            };

            _raycaster.Raycast(ref raycastParams, ref _filter, ref _callback);
            _interactorCore.UpdateCandidate(_callback.Interactable);
        }

        public void Interact()
        {
            _interactorCore.TryInteract();
        }

        public void Release()
        {
            _interactorCore.Release();
        }

        public void Dispose()
        {
            _input.OnClickAction -= HandleInput;
            _interactorCore.OnPoint -= HandlePoint;
            _interactorCore.OnInteract -= HandleInteract;
            _levelManager.OnPlayerCreated -= InitPlayerCamera;
            
            _gameLoop?.RemoveFromLoop(GameLoopType.Awake, this);
            _gameLoop?.RemoveFromLoop(GameLoopType.Update, this);
        }
    }
}