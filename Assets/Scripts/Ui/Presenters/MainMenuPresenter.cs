using System;
using Core;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using R3;

namespace Ui
{
    public class MainMenuPresenter : IUIPresenter, IDisposable
    {
        private const string MainMenuViewId = "MainMenuView";
        private const string SettingViewId = "SettingsView";
        private readonly CompositeDisposable _compositeDisposable;
        
        private readonly MainMenuModel _mainMenuModel;
        private readonly IFactorySystem _factorySystem;
        private readonly IGameStateManager _gameStateManager;
        private readonly IUISystem _uiSystem;
        
        private IMainMenuView _mainMenuView;

        public MainMenuPresenter(MainMenuModel mainMenuModel, IGameStateManager gameStateManager, IFactorySystem factorySystem, IUISystem uiSystem)
        {
            _mainMenuModel = mainMenuModel;
            _gameStateManager = gameStateManager;
            _factorySystem = factorySystem;
            _uiSystem = uiSystem;

            _compositeDisposable = new CompositeDisposable();
        }

        public async UniTask Initialize()
        {
            _mainMenuView = await _factorySystem.CreateFromInterface<IMainMenuView>(MainMenuViewId);
            await _uiSystem.ParentUnderCanvas(_mainMenuView, CanvasType.Windows);
            _uiSystem.RegisterView(MainMenuViewId, _mainMenuView);
            _mainMenuView.Hide();

            _gameStateManager.OnGameStateChanged.Subscribe(DisplayView)
                .AddTo(_compositeDisposable);
            
            _mainMenuView
                .OnButtonClicked
                .Subscribe(async(x) => await HandleViewClick(x))
                .AddTo(_compositeDisposable);
        }

        private async UniTask HandleViewClick(ButtonEvent eventToHandle)
        {
            var eventType = eventToHandle.EventType;
            
            switch (eventType)
            {
                case ButtonEventType.StartNewGame:
                    await OnStartNewGameClicked();
                    break;
                case ButtonEventType.LoadGame:
                    break;
                case ButtonEventType.Settings:
                    await ShowSettings();
                    break;
                case ButtonEventType.Exit:
                    await _mainMenuModel.ExitGame();
                    break;
                default:
                    Logger.Instance.Log($"Unknown button event type {eventType}");
                    break;
            }
        }

        private void DisplayView(GameState state)
        {
            if (state == GameState.MainMenu)
            {
                _mainMenuView.EnableButtonNewGame(true);
                _mainMenuView.Show();
            }
            else
            {
                _mainMenuView.Hide();
            }
        }

        public async UniTask ShowSettings()
        {
            await _uiSystem.ShowViewById(SettingViewId);
        }

        public async UniTask OnStartNewGameClicked()
        {
            _mainMenuView.EnableButtonNewGame(false);
            await _mainMenuModel.StartNewGame();
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}