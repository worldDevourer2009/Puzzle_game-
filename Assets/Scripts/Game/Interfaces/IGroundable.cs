using Core;

namespace Game
{
    public interface IGroundable
    {
        bool IsGrounded(IEntity entity, RaycastParams @params);
    }
}