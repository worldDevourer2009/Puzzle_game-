namespace Core
{
    public enum ActivationState
    {
        Active,
        Inactive,
    }
    
    public interface IActivatable
    {
        ActivationState State { get; }
        void Activate();
        void Deactivate();
    }
}