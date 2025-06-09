using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public interface IPauseMenuView : IUIView
    {
        event Action OnDestroyed;
        event Action<PauseMenuButtonAction> OnPauseMenuClicked;
    }

    public enum PauseMenuButtonAction
    {
        None,
        Resume,
        MainMenu,
        Settings
    }
    
    public class PauseMenuView : MonoBehaviour, IPauseMenuView
    {
        public bool IsVisible => gameObject.activeInHierarchy;
        public event Action<PauseMenuButtonAction> OnPauseMenuClicked;

        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _goToMainMenuButton;
        
        private Action _onClickResumeAction;
        private Action _onClickMainMenuAction;
        private Action _onClickSettings;
        public event Action OnDestroyed;

        private void Awake()
        {
            _onClickResumeAction = () => OnPauseMenuClicked?.Invoke(PauseMenuButtonAction.Resume);
            _onClickMainMenuAction = () => OnPauseMenuClicked?.Invoke(PauseMenuButtonAction.MainMenu);
            _onClickSettings = () => OnPauseMenuClicked?.Invoke(PauseMenuButtonAction.Settings);
            
            _resumeButton.onClick.AddListener(() =>
            {
                _onClickResumeAction.Invoke();
            });
            
            _goToMainMenuButton.onClick.AddListener(() => 
            {
                _onClickMainMenuAction.Invoke();
            });
            
            _settingsButton.onClick.AddListener(() => 
            {
                _onClickSettings.Invoke();
            });
        }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
            _resumeButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
            _goToMainMenuButton.onClick.RemoveAllListeners();
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void Parent(Transform parent)
        {
            transform.SetParent(parent, false);
        }
    }
}