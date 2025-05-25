using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public interface IPauseResumeView
    {
        event Action OnDestroyed;
        event Action<PauseMenuButtonAction> OnPauseMenuClicked;
        void EnableView(bool enable);
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
        public event Action<PauseMenuButtonAction> OnPauseMenuClicked;

        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _goToMainMenuButton;
        [SerializeField] private Image _image;
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

        public void EnableView(bool enable)
        {
            if (_image != null)
            {
                _image.gameObject.SetActive(enable);
            }
        }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
            _resumeButton.onClick.RemoveListener(_onClickResumeAction);
            _goToMainMenuButton.onClick.RemoveListener(_onClickMainMenuAction);
        }
    }
}