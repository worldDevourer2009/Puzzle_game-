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
        event Action<IInteractable> OnStopPoint;
        void Interact();
        void Release();
    }

    public class PlayerInteractor : IPlayerInteractor, IUpdatable, IDisposable
    {
        public event Action<IInteractable> OnInteract;
        public event Action<IInteractable> OnPoint;
        public event Action<IInteractable> OnStopPoint;

        private readonly IPlayerInputHandler _input;
        private readonly IRaycaster _raycaster;
        private readonly PlayerInteractionConfig _playerInteractionConfig;
        private readonly ICameraManager _cameraManager;
        private readonly IGameLoop _gameLoop;
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
            
            _input.OnUseAction += HandleUse;
            
            _input.OnReleaseAction += HandleRelease;
            
            _interactorCore.OnPoint += HandlePoint;
            _interactorCore.OnStopPoint += HandleStopPoint;
            _interactorCore.OnInteract += HandleInteract;

            _interactionMask = _playerInteractionConfig.PlayerInteraction.LayerMask;
            _interactonDistance = _playerInteractionConfig.PlayerInteraction.InteractionDistance;
            
            _gameLoop.AddToGameLoop(GameLoopType.Update, this);
        }

        private void InitPlayerCamera()
        {
            _mainCamera = _cameraManager.GetPlayerCamera();
        }

        private void HandleInteract(IInteractable candidate)
        {
            OnInteract?.Invoke(candidate);
        }

        private void HandlePoint(IInteractable candidate)
        {
            if (candidate != null)
            {
                candidate.Outline();
                OnPoint?.Invoke(candidate);
            }
        }

        private void HandleStopPoint(IInteractable candidate)
        {
            if (candidate != null)
            {
                candidate.ResetOutline();
                OnStopPoint?.Invoke(candidate);
            }
        }

        private void HandleRelease()
        {
            Release();
        }

        private void HandleUse()
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
            
            _callback.Interactable = null;

            _raycaster.Raycast(ref raycastParams, ref _filter, ref _callback);
            _interactorCore.UpdateCandidate(_callback.Interactable);
        }

        private void CheckInteractableBeforeUse()
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
            
            _callback.Interactable = null;

            _raycaster.Raycast(ref raycastParams, ref _filter, ref _callback);
            _interactorCore.UpdateCandidateUse(_callback.Interactable);
        }

        public void Interact()
        {
            CheckInteractableBeforeUse();
            _interactorCore.TryInteract();
        }

        public void Release()
        {
            _interactorCore.Release();
        }

        public void Dispose()
        {
            _input.OnUseAction -= HandleUse;
            _interactorCore.OnPoint -= HandlePoint;
            _interactorCore.OnInteract -= HandleInteract;
        }
    }
}