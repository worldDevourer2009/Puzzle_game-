using System;
using Core;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using CompositeDisposable = R3.CompositeDisposable;

namespace Ui
{
    public class PauseMenuPresenter : IUIPresenter,  IDisposable
    {
        private const string PauseResumeViewId = "PauseResumeView";
        
        private readonly CompositeDisposable _compositeDisposable;
        private readonly PauseResumeModel _pauseResumeModel;
        private readonly IGameStateManager _gameStateManager;
        private readonly IFactorySystem _factorySystem;
        private readonly IUISystem _uiSystem;
        private readonly IInput _input;
        private bool _isPaused;
        
        private IPauseResumeView _pauseResumeView;

        public PauseMenuPresenter(PauseResumeModel pauseResumeModel, IFactorySystem factorySystem, IGameStateManager gameStateManager, IUISystem uiSystem, IInput input)
        {
            _pauseResumeModel = pauseResumeModel;
            _factorySystem = factorySystem;
            _gameStateManager = gameStateManager;
            _uiSystem = uiSystem;
            _input = input;
            
            _compositeDisposable = new CompositeDisposable();
        }
        
        public async UniTask Initialize()
        {
            _pauseResumeView = await _factorySystem.CreateFromInterface<IPauseResumeView>(PauseResumeViewId);
            var canvas = _uiSystem.GetCanvasByType(CanvasType.Windows);
            _pauseResumeView.Parent(canvas.transform);
            _pauseResumeView.Hide();
            
            _pauseResumeView.OnDestroyed += Dispose;
            _pauseResumeView.OnPauseMenuClicked += HandleViewClick;
            _input.OnPauseClicked += HandleClick;
            
            _gameStateManager.OnGameStateChanged.Subscribe(HandleViewDisplay)
                .Dispose();
        }

        private void HandleClick()
        {
            if (!_isPaused)
            {
                _pauseResumeModel.Pause().Forget();
                _pauseResumeView.Show();
            }
            else
            {
                _pauseResumeView.Hide();
                _pauseResumeModel.Resume().Forget();
            }
        }

        private void HandleViewDisplay(GameState state)
        {
            Logger.Instance.Log($"Recived state {state}");
            
            if (_pauseResumeView == null)
            {
                Logger.Instance.Log("View is null");
                return;
            }
            
            switch (state)
            {
                case GameState.Resume:
                    _pauseResumeView.Hide();
                    break;
                case GameState.Playing:
                    _pauseResumeView.Hide();
                    break;
                case GameState.Default:
                case GameState.MainMenu:
                case GameState.NewGame:
                case GameState.LoadGame:
                    _pauseResumeView.Hide();
                    break;
                default:
                    break;
            }
        }

        private async void HandleViewClick(PauseMenuButtonAction action)
        {
            switch (action)
            {
                case PauseMenuButtonAction.Resume:
                    _pauseResumeView.Hide();
                    await _pauseResumeModel.Resume();
                    break;
                case PauseMenuButtonAction.MainMenu:
                    _pauseResumeView.Hide();
                    await _pauseResumeModel.GoToMainMenu();
                    break;
                case PauseMenuButtonAction.Settings:
                case PauseMenuButtonAction.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        public void Dispose()
        {
            _input.OnPauseClicked -= HandleClick;
            _compositeDisposable.Dispose();
            
            if (_pauseResumeView != null)
            {
                _pauseResumeView.OnPauseMenuClicked -= HandleViewClick;
                _pauseResumeView.OnDestroyed -= Dispose;
            }
        }
    }
}