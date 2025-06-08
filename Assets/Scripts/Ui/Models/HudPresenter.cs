using System;
using Cysharp.Threading.Tasks;
using Core;
using Cysharp.Threading.Tasks.Linq;
using R3;

namespace Ui
{
    public class HudPresenter : IUIPresenter, IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable;
        private const string HudViewId = "HudView";
        
        private readonly HudModel _hudModel;
        private readonly IGameStateManager _gameStateManager;
        private readonly IFactorySystem _factorySystem;
        private readonly IUISystem _uiSystem;
        
        private IHudView _hudView;

        public HudPresenter(HudModel hudModel, IGameStateManager gameStateManager, IFactorySystem factorySystem, IUISystem uiSystem)
        {
            _hudModel = hudModel;
            _gameStateManager = gameStateManager;
            _factorySystem = factorySystem;
            _uiSystem = uiSystem;
            _compositeDisposable = new CompositeDisposable();
        }

        public async UniTask Initialize()
        {
            _hudView = await _factorySystem.CreateFromInterface<IHudView>(HudViewId);
            _uiSystem.RegisterView(HudViewId, _hudView);
            _hudView.Hide();
            await _uiSystem.ParentUnderCanvas(_hudView, CanvasType.Hud);

            _hudModel
                .InteractionText
                .Subscribe(ChangeViewText)
                .AddTo(_compositeDisposable);
            
            _hudModel
                .InteractedText
                .Subscribe(EnableInteractedText)
                .AddTo(_compositeDisposable);

            _gameStateManager
                .OnGameStateChanged
                .Subscribe(HandleDisplayState)
                .AddTo(_compositeDisposable);
        }

        private void EnableInteractedText(bool hasToShow)
        {
            _hudView.EnableReleaseText(hasToShow, _hudModel.GetReleaseText());
        }

        private UniTaskVoid HandleDisplayState(GameState state)
        {
            if (state == GameState.Playing || state == GameState.Resume || state == GameState.NewGame)
            {
                _hudView.Show();
            }
            else
            {
                _hudView.Hide();
                _hudView.HideInteractable();
            }
            
            return default;
        }

        private void ChangeViewText(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                _hudView.DisplayInteractable(text);
            }
            else
            {
                _hudView.HideInteractable();
            }
        }
        
        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}