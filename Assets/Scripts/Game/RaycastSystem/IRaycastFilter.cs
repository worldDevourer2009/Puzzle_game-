using Game;
using UnityEngine;

namespace Core
{
    public interface IRaycastFilter
    {
        bool ShouldHit(in RaycastHit hit);
    }
    
    public struct AlwaysTrueFilter : IRaycastFilter
    {
        public bool ShouldHit(in RaycastHit hit) => true;
    }

    public struct InteractableFilter : IRaycastFilter
    {
        public bool ShouldHit(in RaycastHit hit)
        {
            return hit.collider.TryGetComponent<IInteractable>(out var interactable);
        }
    }
}