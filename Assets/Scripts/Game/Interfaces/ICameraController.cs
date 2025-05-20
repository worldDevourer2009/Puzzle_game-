using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public interface ICameraController
    {
        UniTask InitCamera();
        UniTaskVoid MoveCamera(Vector3 direction, float clamp = 90);
    }
}