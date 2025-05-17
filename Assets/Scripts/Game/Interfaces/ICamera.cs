using Core;
using UnityEngine;

namespace Game
{
    public interface ICamera
    {
        CustomCameraType CameraType { get; }
        Camera Camera { get; }
    }
}