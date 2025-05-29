using Core;
using Cysharp.Threading.Tasks;

namespace Ui
{
    public class UIInitializer
    {
        private const string GroupName = "InitControllers";
        
        private readonly IAsyncGroupLoader _asyncGroupLoader;
        
        private readonly LoadingPresenter _startNewGameView;
        private readonly PauseMenuPresenter _pauseMenuPresenter;
        private readonly SettingsPresenter _settingsPresenter;
        private readonly StartNewGamePresenter _startNewGamePresenter;

        private bool _initialized;

        public UIInitializer(IAsyncGroupLoader asyncGroupLoader, LoadingPresenter startNewGameView, PauseMenuPresenter pauseMenuPresenter, SettingsPresenter settingsPresenter, StartNewGamePresenter startNewGamePresenter)
        {
            _asyncGroupLoader = asyncGroupLoader;
            _startNewGameView = startNewGameView;
            _pauseMenuPresenter = pauseMenuPresenter;
            _settingsPresenter = settingsPresenter;
            _startNewGamePresenter = startNewGamePresenter;
            CreateGroupToInitializeControllers();
        }

        public async UniTask InitializeUI()
        {
            await _asyncGroupLoader.RunGroup(GroupName);
        }

        private void CreateGroupToInitializeControllers()
        {
            _asyncGroupLoader.CreateGroup(AsyncGroupType.Parallel, GroupName);
            _asyncGroupLoader.AddToGroup(GroupName, () => _startNewGameView.Initialize(), false);
            _asyncGroupLoader.AddToGroup(GroupName, () => _pauseMenuPresenter.Initialize(), false);
            _asyncGroupLoader.AddToGroup(GroupName, () => _settingsPresenter.Initialize(), false);
            _asyncGroupLoader.AddToGroup(GroupName, () => _startNewGamePresenter.Initialize(), false);
        }
    }
}