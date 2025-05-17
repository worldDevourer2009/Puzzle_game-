using Cysharp.Threading.Tasks;

namespace Core
{
    public interface ISceneCommandsFactory
    {
        UniTask<ICommand> CreateCommand(string sceneName, LoadMode loadMode);
    }
    
    public class SceneCommandsFactory : ISceneCommandsFactory
    {
        private readonly ISceneLoader _sceneLoader;

        public SceneCommandsFactory(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public UniTask<ICommand> CreateCommand(string sceneName, LoadMode loadMode)
        {
            return UniTask.FromResult<ICommand>(new LoadSceneCommand(sceneName, _sceneLoader, loadMode));
        }
    }
}