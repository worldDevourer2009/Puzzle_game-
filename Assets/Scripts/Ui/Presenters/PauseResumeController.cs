using System;
using Core;
using R3;
using CompositeDisposable = R3.CompositeDisposable;

namespace Ui
{
    public class PauseResumePresenter : IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable;
        private readonly PauseResumeModel _pauseResumeModel;
        private readonly IPauseResumeView _pauseResumeView;

        public PauseResumePresenter(PauseResumeModel pauseResumeModel, IPauseResumeView pauseResumeView)
        {
            _pauseResumeModel = pauseResumeModel;
            _pauseResumeView = pauseResumeView;
            _compositeDisposable = new CompositeDisposable();

            _pauseResumeView.OnDestroyed += Dispose;
            _pauseResumeView.OnPauseMenuClicked += HandleViewClick;
            
            _pauseResumeModel.CurrentState
                .Subscribe(HandleViewDisplay)
                .AddTo(_compositeDisposable);
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
            _compositeDisposable.Dispose();
            _pauseResumeView.OnPauseMenuClicked -= HandleViewClick;
            _pauseResumeView.OnDestroyed -= Dispose;
        }
    }
}