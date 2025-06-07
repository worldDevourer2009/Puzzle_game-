using TMPro;
using UnityEngine;

namespace Ui
{
    public interface IHudView : IUIView
    {
        void DisplayInteractable(string text);
        void HideInteractable();
        void EnableReleaseText(bool enable, string text = "");
    }

    public class HudView : MonoBehaviour, IHudView
    {
        public bool IsVisible => gameObject.activeInHierarchy;

        [Header("Core componenets")]
        [SerializeField] private TextMeshProUGUI _textField;
        [SerializeField] private TextMeshProUGUI _releaseText;
        
        [Header("Text offsets")]
        [SerializeField] private float _textOffsetY = -40f;
        [SerializeField] private float _textOffsetX = 50f;

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

        public void DisplayInteractable(string text)
        {
            if (!TrySetTextPosition())
            {
                return;
            }

            _textField.text = text;
        }

        private bool TrySetTextPosition()
        {
            var rectTransform = _textField.GetComponent<RectTransform>();
            
            if (!rectTransform)
            {
                return false;
            }
            
            var canvas = GetComponentInParent<Canvas>();
            if (!canvas)
            {
                return false;
            }
            
            var centerPosition = Vector2.zero;
            centerPosition.x += _textOffsetX;
            centerPosition.y += _textOffsetY;
            
            rectTransform.anchoredPosition = centerPosition;
            
            return true;
        }
        
        public void EnableReleaseText(bool enable, string text = "")
        {
            if (enable)
            {
                _releaseText.text = text;
            }
            else
            {
                _releaseText.text = "";
            }
        }

        public void HideInteractable()
        {
            _textField.text = "";
        }
    }
}