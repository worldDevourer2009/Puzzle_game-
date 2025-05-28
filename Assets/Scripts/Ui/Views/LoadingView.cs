using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Ui
{
    public interface ILoadingView : IUIView
    {
        void DisplayLoadingScreen();
        void HideLoadingScreen();
    }
    
    public class LoadingView : MonoBehaviour, ILoadingView
    {
        public bool IsVisible => gameObject.activeInHierarchy;

        [Header("Core")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _loadingImage;

        [Header("LoadingImage tween comps")]
        [SerializeField] private float _circleRotationRation;
        [SerializeField] private Ease _rotationEase;

        private RectTransform _loadingImageRect;

        public void DisplayLoadingScreen()
        {
            if (_loadingImageRect == null && _loadingImage != null)
            {
                _loadingImageRect = _loadingImage.GetComponent<RectTransform>();
                _loadingImageRect?.DOKill();
            }
            
            if (_backgroundImage != null)
            {
                _backgroundImage.gameObject.SetActive(true);
            }

            if (_loadingImageRect == null)
            {
                return;
            }
            
            _loadingImageRect?.DOLocalRotate(new Vector3(0f, 0f, -360f), _circleRotationRation, RotateMode.FastBeyond360)
                .SetEase(_rotationEase)
                .SetLoops(-1);
        }

        public void HideLoadingScreen()
        {
            if (_backgroundImage != null)
            {
                _backgroundImage?.gameObject.SetActive(true);
            }
            
            if (_loadingImageRect != null)
            {
                _loadingImageRect?.DOKill();
            }
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