using System;
using Core;
using Game;
using R3;

namespace Ui
{
    public class HudModel : IDisposable
    {
        public ReactiveProperty<string> InteractionText => _interactionText;
        public ReactiveProperty<bool> InteractedText => _interactedText;
        
        private readonly ReactiveProperty<string> _interactionText;
        private readonly ReactiveProperty<bool> _interactedText; 
        private readonly IInteractorCore _interactorCore;
        private readonly IInputDataHolder _dataHolder;
        
        private bool _isInteracting;

        public HudModel(IInteractorCore interactorCore, IInputDataHolder dataHolder)
        {
            _interactorCore = interactorCore;
            _dataHolder = dataHolder;
            
            _interactionText = new ReactiveProperty<string>();
            _interactedText = new ReactiveProperty<bool>();
            
            _interactorCore.OnPoint += DisplayInteraction;
            _interactorCore.OnStopPoint += HideInteraction;
            _interactorCore.OnInteract += HandleInteracted;
            _interactorCore.OnStopInteract += HandleStopInteraction;
        }

        private void HandleStopInteraction(IInteractable interactable)
        {
            _interactedText.Value = false;
            _isInteracting = false;
        }

        private void HandleInteracted(IInteractable interactable)
        {
            if (interactable != null)
            {
                _interactedText.Value = true;
                _isInteracting = true;
            }
        }

        private void DisplayInteraction(IInteractable interactable)
        {
            if (interactable != null && !_isInteracting)
            {
                _interactionText.Value = $"Press {_dataHolder.GetKeyBinding(InputAction.Use)} to interact";
            }
        }

        private void HideInteraction(IInteractable interactable)
        {
            if (interactable != null)
            {
                _interactionText.Value = "";
            }
        }

        public string GetReleaseText()
        {
            return $"Press {_dataHolder.GetKeyBinding(InputAction.Release)} to release";
        }

        public void Dispose()
        {
            _interactorCore.OnPoint -= DisplayInteraction;
            _interactorCore.OnStopPoint -= HideInteraction;
            _interactorCore.OnInteract -= HandleInteracted;
            _interactorCore.OnStopInteract -= HandleStopInteraction;
        }
    }
}