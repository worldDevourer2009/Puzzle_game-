using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Logger = Core.Logger;

namespace Ui
{
    public interface IUISystem
    {
        UniTask InitializeUiSystem();
        Canvas GetCanvasByType(CanvasType type);
        UniTask ParentUnderCanvas(Transform objectToParent, CanvasType type);
        UniTask ParentUnderCanvas(IUIView view, CanvasType type);
        UniTask<GameObject> ShowView(string id, CanvasType type);
        UniTask<IUIView> ShowViewFromInterface(string id, CanvasType type);
        void HideView(string id, GameObject view);
    }

    public sealed class UISystem : IUISystem
    {
        private const string CanvasUIPrefabId = "UIRoot";

        public UI UIRoot => _uiRoot;
        
        private readonly IFactorySystem _factorySystem;
        private readonly Dictionary<string, GameObject> _activeViews;

        private UniTaskCompletionSource _completionSource;
        private UI _uiRoot;
        private bool _initializing;

        public UISystem(IFactorySystem factorySystem)
        {
            _factorySystem = factorySystem;
            _activeViews = new Dictionary<string, GameObject>();
            _completionSource = new UniTaskCompletionSource();
        }

        public async UniTask InitializeUiSystem()
        {
            if (_initializing)
            {
                await UniTask.WaitUntil(() => !_initializing);
                return;
            }
            
            _initializing = true;
            
            _uiRoot = await _factorySystem.Create<UI>(CanvasUIPrefabId);
            _uiRoot.InitCanvases();
            
            _initializing = false;
        }

        public Canvas GetCanvasByType(CanvasType type)
        {
            return _uiRoot != null ? _uiRoot.GetCanvasByType(type) : null;
        }

        public async UniTask ParentUnderCanvas(Transform objectToParent, CanvasType type)
        {
            if (_uiRoot == null)
            {
                await InitializeUiSystem();
            }
            
            var canvas = _uiRoot.GetCanvasByType(type);
            objectToParent.SetParent(canvas.transform);
        }

        public async UniTask ParentUnderCanvas(IUIView view, CanvasType type)
        {
            if (_uiRoot == null)
            {
                await InitializeUiSystem();
            }

            var canvas = GetCanvasByType(type);
            view.Parent(canvas.transform);
        }

        public async UniTask<GameObject> ShowView(string id, CanvasType type)
        {
            if (_uiRoot == null)
            {
                await InitializeUiSystem();
            }

            var view = await _factorySystem.Create(id);
            await ParentUnderCanvas(view.transform, type);
            view.gameObject.SetActive(true);
            return view;
        }

        public async UniTask<IUIView> ShowViewFromInterface(string id, CanvasType type)
        {
            if (_uiRoot == null)
            {
                await InitializeUiSystem();
            }

            var view = await _factorySystem.CreateFromInterface<IUIView>(id);
            view.Parent(GetCanvasByType(type).transform);
            view.Show();
            return view;
        }

        public void HideView(string id, GameObject view)
        {
            if (_uiRoot == null)
            {
                Logger.Instance.LogWarning("Root is null");
                return;
            }

            if (view == null)
            {
                Logger.Instance.LogWarning("View is null");
                return;
            }
            
            _factorySystem.Release(id, view);
        }
    }
}