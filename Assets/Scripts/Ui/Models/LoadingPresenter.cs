using R3;
using UnityEngine;

namespace Ui
{
    public class LoadingPresenter
    {
        private readonly CompositeDisposable _compositeDisposable;
        private readonly LoadingModel _loadingModel;
        private readonly ILoadingView _loadingView;

        public LoadingPresenter(LoadingModel loadingModel, ILoadingView loadingView)
        {
            _loadingModel = loadingModel;
            _loadingView = loadingView;
            _compositeDisposable = new CompositeDisposable();
            
            _loadingModel.OnLoading
                .Subscribe(HandleDisplay)
                .AddTo(_compositeDisposable);
        }

        private void HandleDisplay(bool loading)
        {
            if (loading)
            {
                _loadingView.DisplayLoadingScreen();
            }
            else
            {
                _loadingView.HideLoadingScreen();
            }
        }
    }
}