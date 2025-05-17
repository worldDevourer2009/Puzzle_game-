namespace Core
{
    public interface ISceneCommand : ICommand
    {
        string SceneName { get; }
    }
}