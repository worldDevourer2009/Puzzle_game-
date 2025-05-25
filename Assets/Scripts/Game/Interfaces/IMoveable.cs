using UnityEngine;

namespace Game
{
    public interface IMoveable
    {
        void Move(Rigidbody obj, float speed, Vector3 direction);
    }
}