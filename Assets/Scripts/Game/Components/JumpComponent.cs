using UnityEngine;

namespace Game
{
    public class JumpComponent : IJumpable
    {
        public JumpComponent()
        {
        }

        public void Jump(Rigidbody obj, float force)
        {
            obj.AddForce(new Vector3(0f, force), ForceMode.Impulse);
        }
    }
}