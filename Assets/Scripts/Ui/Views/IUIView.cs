using UnityEngine;

namespace Ui
{
    public interface IUIView
    {
        void Show();
        void Hide();
        bool IsVisible { get; }
        void Parent(Transform parent);
    }
}