namespace Core
{
    public interface ILevelCommand : ICommand
    {
        string LevelName { get; }
    }
}