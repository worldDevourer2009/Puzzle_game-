using Cysharp.Threading.Tasks;

namespace Core
{
    public class LoadLevelCommand : ILevelCommand
    {
        public string LevelName => _levelName;
        private readonly ILevelManager _levelManager;
        private readonly string _levelName;

        public LoadLevelCommand(ILevelManager levelManager, string levelName)
        {
            _levelManager = levelManager;
            _levelName = levelName;
        }

        public async UniTask ExecuteAsync()
        {
            await _levelManager.LoadLevelByName(_levelName);
        }
    }
}