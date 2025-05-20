using System;
using Game;
using UnityEngine;

namespace Core
{
    public class InteractorCore : IInteractorCore
    {
        public IInteractable Candidate => _candidate;
        public event Action<IInteractable> OnPoint;
        public event Action<IInteractable> OnInteract;
        
        private IInteractable _candidate;

        public void UpdateCandidate(IInteractable newOne)
        {
            _candidate = newOne;
            OnPoint?.Invoke(_candidate);
        }

        public void TryInteract()
        {
            Debug.Log("Trying to interact");
            if (_candidate != null)
            {
                _candidate.Interact();
                OnInteract?.Invoke(_candidate);
            }
        }

        public void Release()
        {
            _candidate?.StopInteraction();
        }
    }
}