using System;
using Game;

namespace Core
{
    public interface IInteractorCore
    {
        event Action<IInteractable> OnPoint;
        event Action<IInteractable> OnInteract;

        IInteractable Candidate { get; }
        void UpdateCandidate(IInteractable newOne);
        void TryInteract();
        void Release();
    }

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