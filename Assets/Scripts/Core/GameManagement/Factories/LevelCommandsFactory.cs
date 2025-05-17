using Cysharp.Threading.Tasks;

namespace Core
{
    public interface ILevelCommandsFactory
    {
        UniTask<ICommand> CreateCommand(string levelName);
    }
    
    public class LevelCommandsFactory : ILevelCommandsFactory
    {
        private readonly ILevelManager _levelManager;

        public LevelCommandsFactory(ILevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        public UniTask<ICommand> CreateCommand(string levelName)
        {
            return UniTask.FromResult<ICommand>(new LoadLevelCommand(_levelManager, levelName));
        }
    }
}