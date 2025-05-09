using UnityEngine;

namespace Game
{
    public class MoveComponent : IMoveable
    {
        public MoveComponent()
        {
        }

        public void Move(GameObject obj, float speed, Vector3 direction)
        {
            obj.transform.position += direction.normalized * speed * Time.deltaTime;
        }
    }
}