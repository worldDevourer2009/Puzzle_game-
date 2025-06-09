using UnityEngine;

namespace Core
{
    public interface ICamera
    {
        CustomCameraType CameraType { get; }
        Camera Camera { get; }
        AudioListener AudioListener {get;}
    }
}