using Core;
using UnityEngine;

namespace Game
{
    public interface ICamera
    {
        CustomCameraType CameraType { get; }
        Camera Camera { get; }
        Vector3 GetCamForwardDirection(Vector3 dir);
    }
}