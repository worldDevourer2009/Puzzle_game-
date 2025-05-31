using UnityEngine;
using Logger = Core.Logger;

namespace Ui
{
    public static class UIViewMonoBehavior
    {
        public static Vector3? GetScale(this IUIView view)
        {
            var monoBehavior = view as MonoBehaviour;

            if (monoBehavior == null)
            {
                Logger.Instance.LogWarning("Can't get scale for view because it's not monobehavior");
                return null;
            }

            return monoBehavior.transform.localScale;
        }

        public static void SetScale(this IUIView view, Vector3 scale)
        {
            var monoBehavior = view as MonoBehaviour;

            if (monoBehavior == null)
            {
                Logger.Instance.LogWarning("Can't set scale for view because it's not monobehavior");
                return;
            }

            monoBehavior.transform.localScale = scale;
        }

        public static float? GetAlpha(this IUIView view)
        {
            var monoBehavior = view as MonoBehaviour;

            if (monoBehavior == null)
            {
                Logger.Instance.LogWarning("Can't get alpha for view because it's not monobehavior");
                return null;
            }

            if (monoBehavior.TryGetComponent<CanvasGroup>(out var group))
            {
                return group.alpha;
            }

            group = monoBehavior.gameObject.AddComponent<CanvasGroup>();
            return group.alpha;
        }

        public static void SetAlpha(this IUIView view, float alpha)
        {
            var monoBehavior = view as MonoBehaviour;

            if (monoBehavior == null)
            {
                Logger.Instance.LogWarning("Can't set alpha for view because it's not monobehavior");
                return;
            }
            
            if (monoBehavior.TryGetComponent<CanvasGroup>(out var group))
            {
                group.alpha = alpha;
            }
            else
            {
                group = monoBehavior.gameObject.AddComponent<CanvasGroup>();
                group.alpha = alpha;
            }
        }
    }
}