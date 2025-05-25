using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "CursorConfig", menuName = "Configs/CursorConfig", order = 4)]
    public class CursorConfig : ScriptableObject
    {
        [SerializeField] public List<CursorSettings> CursorSettingsList;
    }

    [Serializable]
    public class CursorSettings
    {
        public CustomCameraType CameraType;
        public CursorLockMode CursorLockMode;
        public bool IsVisible;
    }
}