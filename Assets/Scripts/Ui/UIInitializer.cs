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
        private readonly MainMenuPresenter _mainMenuPresenter;
        private readonly HudPresenter _hudPresenter;

        private bool _initialized;

        public UIInitializer(IAsyncGroupLoader asyncGroupLoader, LoadingPresenter startNewGameView,
            PauseMenuPresenter pauseMenuPresenter, SettingsPresenter settingsPresenter,
            MainMenuPresenter mainMenuPresenter, HudPresenter hudPresenter)
        {
            _asyncGroupLoader = asyncGroupLoader;
            _startNewGameView = startNewGameView;
            _pauseMenuPresenter = pauseMenuPresenter;
            _settingsPresenter = settingsPresenter;
            _mainMenuPresenter = mainMenuPresenter;
            _hudPresenter = hudPresenter;
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
            _asyncGroupLoader.AddToGroup(GroupName, () => _mainMenuPresenter.Initialize(), false);
            _asyncGroupLoader.AddToGroup(GroupName, () => _hudPresenter.Initialize(), false);
        }
    }
}