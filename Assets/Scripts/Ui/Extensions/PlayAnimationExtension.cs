using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Logger = Core.Logger;

namespace Ui
{
    public static class PlayAnimationExtension
    {
        public static async UniTask PlayFadeAnimation(this IUIView view, float alpha, float duration,
            Ease ease = Ease.Linear)
        {
            var monoBehavior = GetMonoBehavior(view);

            if (monoBehavior == null)
            {
                Logger.Instance.LogWarning("Trying to use static method for IUIview which is not MonoBehavior");
                return;
            }

            if (!monoBehavior.TryGetComponent<CanvasGroup>(out var canvas))
            {
                canvas = monoBehavior.gameObject.AddComponent<CanvasGroup>();
            }

            await canvas.DOFade(alpha, duration).SetEase(ease).AsyncWaitForCompletion().AsUniTask();
        }

        public static async UniTask PlayScaleAnimation(this IUIView view, Vector3 targetScale, float duration,
            Ease ease = Ease.Linear)
        {
            var monoBehavior = GetMonoBehavior(view);

            if (monoBehavior == null)
            {
                Logger.Instance.LogWarning("Trying to use static method for IUIview which is not MonoBehavior");
                return;
            }

            var rect = RectTransform(view);
            await rect.DOScale(targetScale, duration).SetEase(ease).AsyncWaitForCompletion().AsUniTask();
        }

        private static MonoBehaviour GetMonoBehavior(IUIView view)
        {
            var monoBehavior = view as MonoBehaviour;

            if (monoBehavior == null)
            {
                Logger.Instance.LogWarning("Trying to use static method for IUIview which is not MonoBehavior");
                return null;
            }

            return monoBehavior;
        }

        public static RectTransform RectTransform(this IUIView view)
        {
            var monoBehavior = GetMonoBehavior(view);

            if (monoBehavior == null)
            {
                Logger.Instance.LogWarning("Ui View is does not impliment monobehavior");
                return null;
            }
            
            if (!monoBehavior.TryGetComponent<RectTransform>(out var rectTransform))
            {
                rectTransform = monoBehavior.gameObject.AddComponent<RectTransform>();
            }

            return rectTransform;
        }
    }
}