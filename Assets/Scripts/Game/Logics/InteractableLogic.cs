using UnityEngine;

namespace Game
{
    public abstract class InteractableLogic : MonoBehaviour, IInteractable
    {
        public abstract void Interact();
        
        public abstract void StopInteraction();
    }
}