using UnityEngine;

namespace Game
{
    public interface IMoveable
    {
        void Move(GameObject obj, float speed, Vector3 direction);
    }
}