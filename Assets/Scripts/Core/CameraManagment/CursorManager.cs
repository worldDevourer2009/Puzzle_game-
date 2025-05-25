using System;
using UnityEngine;
using ZLinq;

namespace Core
{
    public interface ICursorManager
    {
        void SetActiveCursorState(CustomCameraType type);
    }
    
    public class CursorManager : ICursorManager, IAwakable, IDisposable
    {
        private readonly ICameraManager _cameraManager;
        private readonly CursorConfig _cursorConfig;

        public CursorManager(ICameraManager cameraManager, CursorConfig cursorConfig)
        {
            _cameraManager = cameraManager;
            _cursorConfig = cursorConfig;
        }
        
        public void AwakeCustom()
        {
            _cameraManager.OnCameraChanged += HandleCameraChanged;
        }

        private void HandleCameraChanged()
        {
            var activeType = _cameraManager.GetActiveCameraType();
            SetActiveCursorState(activeType);
        }

        public void SetActiveCursorState(CustomCameraType type)
        {
            if (type == CustomCameraType.Default)
            {
                SetDefaultCursorState();
                return;
            }

            var config = _cursorConfig.CursorSettingsList.AsValueEnumerable().FirstOrDefault(x => x.CameraType == type);

            if (config == null)
            {
                SetDefaultCursorState();
                return;
            }

            Cursor.lockState = config.CursorLockMode;
            Cursor.visible = config.IsVisible;
        }

        private void SetDefaultCursorState()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Dispose()
        {
            _cameraManager.OnCameraChanged -= HandleCameraChanged;
        }
    }
}