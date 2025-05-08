using UnityEngine;

namespace Game
{
    public interface IJumpable
    {
        void Jump(Rigidbody obj, float force);
    }
}