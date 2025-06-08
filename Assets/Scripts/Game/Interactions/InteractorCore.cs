using System;
using Game;

namespace Core
{
    public class InteractorCore : IInteractorCore
    {
        public IInteractable Candidate => _candidate;
        public event Action<IInteractable> OnPoint;
        public event Action<IInteractable> OnStopInteract;
        public event Action<IInteractable> OnInteract;
        public event Action<IInteractable> OnStopPoint;
        
        private IInteractable _candidate;
        private IInteractable _candidateUse;
        private IInteractable _currentInteractedItem;

        public void UpdateCandidate(IInteractable newOne)
        {
            if (_candidate != newOne)
            {
                var previousCandidate = _candidate;
                _candidate = newOne;
                
                if (previousCandidate != null)
                {
                    if (IsValidInteractable(previousCandidate))
                    {
                        previousCandidate.ResetOutline();
                    }
                    
                    OnStopPoint?.Invoke(previousCandidate);
                }
                
                if (_candidate != null)
                {
                    OnPoint?.Invoke(_candidate);
                }
            }
        }

        public void UpdateCandidateUse(IInteractable newOne)
        {
            if (newOne != null && newOne != _candidateUse && newOne != _currentInteractedItem)
            {
                _candidateUse = newOne;
            }
            else if (newOne == null)
            {
                _candidateUse = null;
            }
        }

        public void TryInteract()
        {
            if (_candidateUse != null && _candidateUse != _currentInteractedItem)
            {
                Logger.Instance.LogWarning("Interacting");
                _candidateUse.Interact();
                
                if (_candidateUse.Type == InteractableType.Usable)
                {
                    _currentInteractedItem = _candidateUse;
                    OnInteract?.Invoke(_candidateUse);
                }
            }

            if (_candidateUse == null)
            {
                _candidateUse = null;
            }

            if (_currentInteractedItem == null)
            {
                _currentInteractedItem = null;
            }
        }

        public void Release()
        {
            if (_currentInteractedItem != null)
            {
                _currentInteractedItem.StopInteraction();
                var itemToNotify = _currentInteractedItem;
                _currentInteractedItem = null;
                _candidateUse = null;
                OnStopInteract?.Invoke(itemToNotify);
            }
        }
        
        private bool IsValidInteractable(IInteractable interactable)
        {
            if (interactable is UnityEngine.MonoBehaviour mono)
            {
                return mono != null && mono;
            }
            
            return interactable != null;
        }
    }
}