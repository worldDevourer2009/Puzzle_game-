using UnityEngine;

namespace Game
{
    public abstract class InteractableComponent : MonoBehaviour, IInteractable
    {
        public abstract void Interact();
        
        public abstract void StopInteraction();
    }
}