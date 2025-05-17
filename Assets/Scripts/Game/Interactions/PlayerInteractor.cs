using System;
using Core;
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

    public class PlayerInteractor : IPlayerInteractor, IUpdatable, IDisposable
    {
        public event Action<IInteractable> OnInteract;
        public event Action<IInteractable> OnPoint;

        private readonly IPlayerInputHandler _input;
        private readonly IGameLoop _gameLoop;
        private readonly IRaycaster _raycaster;
        private readonly PlayerInteractionConfig _playerInteractionConfig;
        private readonly ICameraManager _cameraManager;
        private readonly ILogger _logger;
        private readonly IInteractorCore _interactorCore;

        private Camera _mainCamera;
        private float _interactonDistance;
        private LayerMask _interactionMask;
        
        private InteractableFilter _filter;
        private InteractableCallback _callback;

        public PlayerInteractor(IInteractorCore interactorCore, ICameraManager cameraManager, IPlayerInputHandler input,
            IGameLoop gameLoop,
            IRaycaster raycaster, PlayerInteractionConfig playerInteractionConfig, ILogger logger)
        {
            _interactorCore = interactorCore;
            _cameraManager = cameraManager;
            _input = input;
            _gameLoop = gameLoop;
            _raycaster = raycaster;
            _playerInteractionConfig = playerInteractionConfig;
            _logger = logger;

            _filter = new InteractableFilter();
            _callback = new InteractableCallback();
            
            _input.OnUseAction += HandleInput;
            _interactorCore.OnPoint += HandlePoint;
            _interactorCore.OnInteract += HandleInteract;

            _interactionMask = _playerInteractionConfig.PlayerInteraction.LayerMask;
            _interactonDistance = _playerInteractionConfig.PlayerInteraction.InteractionDistance;

            if (_gameLoop != null)
            {
                _gameLoop.AddToGameLoop(GameLoopType.Update, this);
            }
        }

        private void InitPlayerCamera()
        {
            _mainCamera = _cameraManager.GetPlayerCamera();
        }

        private void HandleInteract(IInteractable candidate)
        {
            OnInteract?.Invoke(candidate);
        }

        private void HandlePoint(IInteractable candid)
        {
            OnPoint?.Invoke(candid);
        }

        private void HandleInput()
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
                InitPlayerCamera();
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
            _input.OnUseAction -= HandleInput;
            _interactorCore.OnPoint -= HandlePoint;
            _interactorCore.OnInteract -= HandleInteract;
            
            _gameLoop?.RemoveFromLoop(GameLoopType.Update, this);
        }
    }
}