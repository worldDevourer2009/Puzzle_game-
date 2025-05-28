using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public interface IPauseResumeView : IUIView
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
    
    public class PauseResumeView : MonoBehaviour, IPauseResumeView
    {
        public bool IsVisible => gameObject.activeInHierarchy;
        public event Action<PauseMenuButtonAction> OnPauseMenuClicked;

        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _goToMainMenuButton;
        private UnityAction _onClickResumeAction;
        private UnityAction _onClickMainMenuAction;
        public event Action OnDestroyed;

        private void Awake()
        {
            _onClickResumeAction = () => OnPauseMenuClicked?.Invoke(PauseMenuButtonAction.Resume);
            _onClickMainMenuAction = () => OnPauseMenuClicked?.Invoke(PauseMenuButtonAction.MainMenu);
            
            _resumeButton.onClick.AddListener(_onClickResumeAction);
            _goToMainMenuButton.onClick.AddListener(_onClickMainMenuAction);
        }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
            _resumeButton.onClick.RemoveListener(_onClickResumeAction);
            _goToMainMenuButton.onClick.RemoveListener(_onClickMainMenuAction);
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
            transform.SetParent(parent);
        }
    }
}