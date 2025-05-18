using System;
using Core;
using R3;
using UnityEngine;
using CompositeDisposable = R3.CompositeDisposable;

namespace Ui
{
    public class PauseResumeController : IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable;
        private readonly PauseResumeModel _pauseResumeModel;
        private readonly IPauseResumeView _pauseResumeView;

        public PauseResumeController(PauseResumeModel pauseResumeModel, IPauseResumeView pauseResumeView)
        {
            _pauseResumeModel = pauseResumeModel;
            _pauseResumeView = pauseResumeView;
            _compositeDisposable = new CompositeDisposable();
            
            _pauseResumeView.OnResumeClicked += HandleViewClick;
            
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

        private async void HandleViewClick()
        {
            await _pauseResumeModel.Resume();
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
            _pauseResumeView.OnResumeClicked -= HandleViewClick;
        }
    }
}