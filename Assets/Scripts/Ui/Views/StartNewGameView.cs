using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Ui
{
    public interface IStartNewGameView : IUIView
    {
        void EnableButton(bool enable);
    }
    
    public class StartNewGameView : MonoBehaviour, IStartNewGameView
    {
        public bool IsVisible => gameObject.activeInHierarchy;
        
        [SerializeField] private Button _button;
        private StartNewGamePresenter _startNewGamePresenter;
        
        [Inject]
        public void Construct(StartNewGamePresenter startNewGamePresenter)
        {
            _startNewGamePresenter = startNewGamePresenter;
        }

        public void Start()
        {
            _button.interactable = true;
            _button.onClick.AddListener(HandleClick);
        }

        private void HandleClick()
        {
            _startNewGamePresenter.OnButtonClicked().Forget();
        }

        public void EnableButton(bool enable)
        {
            _button.interactable = enable;
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