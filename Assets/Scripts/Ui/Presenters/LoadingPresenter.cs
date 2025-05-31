using System;
using Core;
using Cysharp.Threading.Tasks;
using R3;

namespace Ui
{
    public class LoadingPresenter : IUIPresenter, IDisposable
    {
        private const string LoadingViewId = "LoadingView";
        
        private readonly CompositeDisposable _compositeDisposable;
        private readonly IFactorySystem _factorySystem;
        private readonly IUISystem _uiSystem;
        private readonly ISceneLoader _sceneLoader;
        
        private ILoadingView _loadingView;

        public LoadingPresenter(ISceneLoader sceneLoader, IFactorySystem factorySystem, IUISystem uiSystem)
        {
            _sceneLoader = sceneLoader;
            _factorySystem = factorySystem;
            _uiSystem = uiSystem;
            
            _compositeDisposable = new CompositeDisposable();
        }
        
        public async UniTask Initialize()
        {
            _loadingView = await _factorySystem.CreateFromInterface<ILoadingView>(LoadingViewId);
            _loadingView.Hide();
            _uiSystem.RegisterView(LoadingViewId, _loadingView);
            await _uiSystem.ParentUnderCanvas(_loadingView, CanvasType.Windows);
            
            _sceneLoader.OnLoad += DisplayView;
            _sceneLoader.OnLoaded += HideView;
        }

        private void DisplayView()
        {
            _loadingView.Show();
            _loadingView.DisplayLoadingScreen();
        }

        private void HideView()
        {
            _loadingView.HideLoadingScreen();
            _loadingView.Hide();
        }

        public void Dispose()
        {
            _sceneLoader.OnLoad -= DisplayView;
            _sceneLoader.OnLoaded -= HideView;
            _compositeDisposable.Dispose();
        }
    }
}