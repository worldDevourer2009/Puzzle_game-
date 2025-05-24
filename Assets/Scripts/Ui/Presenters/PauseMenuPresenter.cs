using System;
using Core;
using Cysharp.Threading.Tasks;
using R3;
using CompositeDisposable = R3.CompositeDisposable;

namespace Ui
{
    public class PauseMenuPresenter : IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable;
        private readonly PauseResumeModel _pauseResumeModel;
        private readonly IInput _input;
        private readonly IPauseResumeView _pauseResumeView;
        private bool _isPaused;

        public PauseMenuPresenter(PauseResumeModel pauseResumeModel, IPauseResumeView pauseResumeView, IInput input)
        {
            _pauseResumeModel = pauseResumeModel;
            _pauseResumeView = pauseResumeView;
            _input = input;
            
            _compositeDisposable = new CompositeDisposable();

            _pauseResumeView.OnDestroyed += Dispose;
            _pauseResumeView.OnPauseMenuClicked += HandleViewClick;
            _input.OnPauseClicked += HandleClick;
            
            _pauseResumeModel.CurrentState
                .Subscribe(HandleViewDisplay)
                .AddTo(_compositeDisposable);
        }
        
        private void HandleClick()
        {
            if (!_isPaused)
            {
                _pauseResumeModel.Pause().Forget();
            }
            else
            {
                _pauseResumeModel.Resume().Forget();
            }
        }

        private void HandleViewDisplay(GameState state)
        {
            switch (state)
            {
                case GameState.Pause:
                    _pauseResumeView.EnableView(true);
                    break;
                case GameState.Resume:
                    _pauseResumeView.EnableView(false);
                    break;
                case GameState.Playing:
                    _pauseResumeView.EnableView(true);
                    break;
            }
        }

        private async void HandleViewClick(PauseMenuButtonAction action)
        {
            switch (action)
            {
                case PauseMenuButtonAction.Resume:
                    await _pauseResumeModel.Resume();
                    break;
                case PauseMenuButtonAction.MainMenu:
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
            _pauseResumeView.OnPauseMenuClicked -= HandleViewClick;
            _pauseResumeView.OnDestroyed -= Dispose;
        }
    }
}