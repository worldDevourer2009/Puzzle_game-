using UnityEngine;

namespace Game
{
    public class MoveLogic : IMoveable
    {
        public MoveLogic()
        {
        }

        public void Move(Rigidbody rb, float speed, Vector3 direction)
        {
            rb.MovePosition(rb.position + direction.normalized * speed * Time.deltaTime); 
        }
    }
}