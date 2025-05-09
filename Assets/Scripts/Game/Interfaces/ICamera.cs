using UnityEngine;

namespace Game
{
    public interface ICamera
    {
        void MoveCamera(GameObject camera, Vector3 direction, float clamp = 90);
    }
}