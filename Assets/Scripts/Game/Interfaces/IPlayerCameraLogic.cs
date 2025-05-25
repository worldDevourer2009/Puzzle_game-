using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public interface IPlayerCameraLogic
    {
        UniTask InitCamera();
        UniTaskVoid MoveCamera(Vector3 direction);
    }
}