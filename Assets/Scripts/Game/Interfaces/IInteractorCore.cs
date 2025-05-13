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
}