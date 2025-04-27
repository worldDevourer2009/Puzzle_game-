namespace Game
{
    public interface IActivatable
    {
        void Activate(IActivatable activatable);
        void Deactivate(IActivatable activatable);
    }
}