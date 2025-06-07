using System;
using Game;

namespace Core
{
    public class InteractorCore : IInteractorCore
    {
        public IInteractable Candidate => _candidate;
        public event Action<IInteractable> OnPoint;
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
                    previousCandidate.ResetOutline();
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
                _candidateUse.Interact();
                _currentInteractedItem = _candidateUse;
                OnInteract?.Invoke(_candidateUse);
            }
        }

        public void Release()
        {
            if (_currentInteractedItem != null)
            {
                _currentInteractedItem.StopInteraction();
                _currentInteractedItem = null;
                _candidateUse = null;
            }
        }
    }
}