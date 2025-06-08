namespace Game
{
    public enum InteractableType
    {
        Usable,
        Unusable
    }
    
    public interface IInteractable
    {
        InteractableType Type { get; }
        void Interact();
        void StopInteraction();
        void Outline();
        void ResetOutline();
    }
}