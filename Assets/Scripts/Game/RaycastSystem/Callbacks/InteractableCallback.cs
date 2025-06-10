using Core;
using UnityEngine;

namespace Game
{
    public struct InteractableCallback : IRaycastCallback
    {
        public IInteractable Interactable;

        public void OnHit(in RaycastHit hit)
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                Interactable = interactable;
            }
        }
    }
}