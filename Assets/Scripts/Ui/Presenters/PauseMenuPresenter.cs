using System;
using Core;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using CompositeDisposable = R3.CompositeDisposable;
using Logger = Core.Logger;

namespace Ui
{
    public class PauseMenuPresenter : IUIPresenter,  IDisposable
    {
        private const string PauseResumeViewId = "PauseMenuView";
        private const string SettingViewId = "SettingsView";
        
        private readonly CompositeDisposable _compositeDisposable;
        private readonly PauseMenuModel _pauseMenuModel;
        private readonly IGameStateManager _gameStateManager;
        private readonly IFactorySystem _factorySystem;
        private readonly IUISystem _uiSystem;
        private readonly IInput _input;
        private bool _isPaused;
        
        private IPauseMenuView _pauseMenuView;

        public PauseMenuPresenter(PauseMenuModel pauseMenuModel, IFactorySystem factorySystem, IGameStateManager gameStateManager, IUISystem uiSystem, IInput input)
        {
            _pauseMenuModel = pauseMenuModel;
            _factorySystem = factorySystem;
            _gameStateManager = gameStateManager;
            _uiSystem = uiSystem;
            _input = input;
            
            _compositeDisposable = new CompositeDisposable();
        }
        
        public async UniTask Initialize()
        {
            _pauseMenuView = await _factorySystem.CreateFromInterface<IPauseMenuView>(PauseResumeViewId);
            _uiSystem.RegisterView(PauseResumeViewId, _pauseMenuView);
            _pauseMenuView.Hide();
            await _uiSystem.ParentUnderCanvas(_pauseMenuView, CanvasType.Windows);
            
            _pauseMenuView.OnDestroyed += Dispose;
            _pauseMenuView.OnPauseMenuClicked += HandleViewClick;
            _input.OnPauseClicked += HandleClick;
            
            _gameStateManager.OnGameStateChanged.Subscribe(HandleViewDisplay)
                .Dispose();
        }

        private async void HandleClick()
        {
            if (!_isPaused)
            {
                _pauseMenuModel.Pause().Forget();
                var targetScale = _pauseMenuView.GetScale();

                if (targetScale != null)
                {
                    _pauseMenuView.SetScale(Vector3.zero);
                    _pauseMenuView.Show();
                    _pauseMenuView.PlayScaleAnimation((Vector3)targetScale).Forget();
                }
                else
                {
                    _pauseMenuView.Show();
                }
            }
            else
            {
                await HideViewWithAnimation();
                _pauseMenuModel.Resume().Forget();
            }
        }

        private void HandleViewDisplay(GameState state)
        {
            Logger.Instance.Log($"Recived state {state}");
            
            if (_pauseMenuView == null)
            {
                Logger.Instance.Log("View is null");
                return;
            }
            
            switch (state)
            {
                case GameState.Resume:
                    HideViewWithAnimation().Forget();
                    break;
                case GameState.Playing:
                    HideViewWithAnimation().Forget();
                    break;
                case GameState.Default:
                case GameState.MainMenu:
                case GameState.NewGame:
                case GameState.LoadGame:
                    _pauseMenuView.Hide();
                    break;
                default:
                    break;
            }
        }

        private async UniTask HideViewWithAnimation()
        {
            var targetScale = _pauseMenuView.GetScale();
            
            if (targetScale != null)
            {
                await _pauseMenuView.PlayScaleAnimation(Vector3.zero);
                _pauseMenuView.Hide();
                _pauseMenuView.SetScale((Vector3)targetScale);
            }
            else
            {
                _pauseMenuView.Hide();
            }
        }

        private async void HandleViewClick(PauseMenuButtonAction action)
        {
            switch (action)
            {
                case PauseMenuButtonAction.Resume:
                    await HideViewWithAnimation();
                    await _pauseMenuModel.Resume();
                    break;
                case PauseMenuButtonAction.MainMenu:
                    _pauseMenuView.Hide();
                    await _pauseMenuModel.GoToMainMenu();
                    break;
                case PauseMenuButtonAction.Settings:
                    await _uiSystem.ShowViewById(SettingViewId);
                    break;
                case PauseMenuButtonAction.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        public void Dispose()
        {
            _input.OnPauseClicked -= HandleClick;
            _compositeDisposable.Dispose();
            
            if (_pauseMenuView != null)
            {
                _pauseMenuView.OnPauseMenuClicked -= HandleViewClick;
                _pauseMenuView.OnDestroyed -= Dispose;
            }
        }
    }
}