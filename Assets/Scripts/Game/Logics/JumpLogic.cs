using UnityEngine;

namespace Game
{
    public class JumpLogic : IJumpable
    {
        public JumpLogic()
        {
        }

        public void Jump(Rigidbody obj, float force)
        {
            obj.AddForce(new Vector3(0f, force), ForceMode.Impulse);
        }
    }
}