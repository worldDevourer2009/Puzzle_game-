using System;
using Core;
using R3;

namespace Ui
{
    public class LoadingPresenter : IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable;
        private readonly ISceneLoader _sceneLoader;
        private readonly ILoadingView _loadingView;

        public LoadingPresenter(ISceneLoader sceneLoader, ILoadingView loadingView)
        {
            _sceneLoader = sceneLoader;
            _loadingView = loadingView;
            _compositeDisposable = new CompositeDisposable();

            _sceneLoader.OnLoad += DisplayView;
            _sceneLoader.OnLoaded += HideView;
        }

        private void DisplayView()
        {
            _loadingView.DisplayLoadingScreen();
        }

        private void HideView()
        {
            _loadingView.HideLoadingScreen();
        }

        public void Dispose()
        {
            _sceneLoader.OnLoad -= DisplayView;
            _sceneLoader.OnLoaded -= HideView;
            _compositeDisposable.Dispose();
        }
    }
}