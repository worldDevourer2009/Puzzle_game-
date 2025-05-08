using UnityEngine;

namespace Core
{
    public interface IInputSource
    {
        Vector3 GetMoveDirection();
        bool IsJumpPressed();
        bool IsUsePressed();
    }

    public sealed class KeyboardSource : IInputSource
    {
        private readonly InputConfig _inputConfig;

        public KeyboardSource(InputConfig inputConfig)
        {
            _inputConfig = inputConfig;
        }

        public Vector3 GetMoveDirection()
        {
            var z = 0f;
            var x = 0f;

            if (Input.GetKey(_inputConfig.GetKeyboardKey(InputAction.MoveBackward)))
            {
                z -= 1;
            }
            
            if (Input.GetKey(_inputConfig.GetKeyboardKey(InputAction.MoveForward)))
            {
                z += 1;
            }
            
            if (Input.GetKey(_inputConfig.GetKeyboardKey(InputAction.MoveRight)))
            {
                x += 1;
            }
            
            if (Input.GetKey(_inputConfig.GetKeyboardKey(InputAction.MoveLeft)))
            {
                x -= 1;
            }

            var direction = new Vector3(x, 0f, z);
            return direction.sqrMagnitude > 1 ? direction.normalized : direction;
        }
        
        public bool IsJumpPressed()
        {
            if (Input.GetKeyDown(_inputConfig.GetKeyboardKey(InputAction.Jump)))
                return true;

            return false;
        }

        public bool IsUsePressed()
        {
            if (Input.GetKeyDown(_inputConfig.GetKeyboardKey(InputAction.Use)))
                return true;

            return false;
        }
    }
}