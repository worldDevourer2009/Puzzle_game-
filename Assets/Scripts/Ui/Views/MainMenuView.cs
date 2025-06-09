using UnityEngine;
using UnityEngine.UI;
using Zenject;
using R3;

namespace Ui
{
    public enum ButtonEventType
    {
        StartNewGame,
        LoadGame,
        Settings,
        Exit,
    }
    
    public struct ButtonEvent
    {
        public ButtonEventType EventType;
        public object Data;
        
        public ButtonEvent(ButtonEventType eventType, object data = null)
        {
            EventType = eventType;
            Data = data;
        }
    }

    
    public interface IMainMenuView : IUIView
    {
        Observable<ButtonEvent> OnButtonClicked { get; }
        void EnableButtonNewGame(bool enable);
    }
    
    public class MainMenuView : MonoBehaviour, IMainMenuView
    {
        public Observable<ButtonEvent> OnButtonClicked => _buttonClickSubject;
        public bool IsVisible => gameObject.activeInHierarchy;
        
        [SerializeField] private Button _startNewGameButton;
        [SerializeField] private Button _loadGameButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;
        
        private readonly Subject<ButtonEvent> _buttonClickSubject = new();
        
        [Inject]
        public void Construct()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            _startNewGameButton.interactable = true;
            _startNewGameButton.onClick.AddListener(() => HandleClick(ButtonEventType.StartNewGame));
            
            _loadGameButton.interactable = true;
            _loadGameButton.onClick.AddListener(() => HandleClick(ButtonEventType.LoadGame));
            
            _settingsButton.interactable = true;
            _settingsButton.onClick.AddListener(() => HandleClick(ButtonEventType.Settings));
            
            _exitButton.interactable = true;
            _exitButton.onClick.AddListener(() => HandleClick(ButtonEventType.Exit));
        }
        
        private void HandleClick(ButtonEventType eventType, object data = null)
        {
            _buttonClickSubject.OnNext(new ButtonEvent(eventType, data));
        }

        public void EnableButtonNewGame(bool enable)
        {
            _startNewGameButton.interactable = enable;
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