using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
    public enum CanvasType
    {
        Windows,
        Hud,
        Hints,
        Popups
    }
    
    [Serializable]
    public class TypedCanvas
    {
        public Canvas Canvas;
        public CanvasType CanvasType;
    }
    
    public class UIRoot : MonoBehaviour
    {
        [SerializeField] private TypedCanvas _windows;
        [SerializeField] private TypedCanvas _hud;
        [SerializeField] private TypedCanvas _hints;
        [SerializeField] private TypedCanvas _popups;
        
        private Dictionary<CanvasType, Canvas> _typedCanvas;

        public void InitCanvases()
        {
            _typedCanvas = new Dictionary<CanvasType, Canvas>()
            {
                { _windows.CanvasType, _windows.Canvas },
                { _hud.CanvasType, _hud.Canvas },
                { _hints.CanvasType, _hints.Canvas },
                { _popups.CanvasType, _popups.Canvas },
            };
        }

        public Canvas GetCanvasByType(CanvasType type)
        {
            return _typedCanvas.GetValueOrDefault(type);
        }
    }
}