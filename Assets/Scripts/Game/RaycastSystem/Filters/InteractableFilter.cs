using Core;
using UnityEngine;

namespace Game
{
    public struct InteractableFilter : IRaycastFilter
    {
        public bool ShouldHit(in RaycastHit hit)
        {
            return hit.collider.TryGetComponent<IInteractable>(out var interactable);
        }
    }
}