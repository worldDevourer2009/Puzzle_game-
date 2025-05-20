using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Ui
{
    public interface IStartNewGameView
    {
        void DisableButton();
    }
    
    [RequireComponent(typeof(Button))]
    public class StartNewGameView : MonoBehaviour, IStartNewGameView
    {
        private Button _button;
        private StartNewGamePresenter _startNewGamePresenter;
        
        [Inject]
        public void Construct(StartNewGamePresenter startNewGamePresenter)
        {
            _startNewGamePresenter = startNewGamePresenter;
            _button = GetComponent<Button>();
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

        public void DisableButton()
        {
            _button.interactable = false;
        }
    }
}