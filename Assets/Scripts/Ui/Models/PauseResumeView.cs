using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public interface IPauseResumeView
    {
        event Action OnResumeClicked;
        void EnableView(bool enable);
    }
    
    public class PauseResumeView : MonoBehaviour, IPauseResumeView
    {
        public event Action OnResumeClicked;

        [SerializeField] private Button _button;
        [SerializeField] private Image _image;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnResumeClicked?.Invoke());
        }

        public void EnableView(bool enable)
        {
            this.gameObject.SetActive(enable);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(() => OnResumeClicked?.Invoke());
        }
    }
}