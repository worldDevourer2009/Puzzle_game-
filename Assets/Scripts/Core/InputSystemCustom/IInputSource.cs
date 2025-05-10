using UnityEngine;

namespace Core
{
    public interface IInputSource
    {
        Vector3 GetMoveDirection();
        bool IsJumpPressed();
        bool IsUsePressed();
        bool IsRunning();
        bool IsClicked(out Vector3 pos);
    }

    public sealed class KeyboardSource : IInputSource
    {
        private readonly InputConfig _inputConfig;
        private readonly IRaycaster _raycaster;

        public KeyboardSource(InputConfig inputConfig, IRaycaster raycaster)
        {
            _inputConfig = inputConfig;
            _raycaster = raycaster;
        }

        public Vector3 GetMoveDirection()
        {
            var direction = Vector3.zero;
            
            direction += GetDirection(InputAction.MoveForward, Vector3.forward);
            direction += GetDirection(InputAction.MoveBackward, Vector3.back);
            direction += GetDirection(InputAction.MoveRight, Vector3.right);
            direction += GetDirection(InputAction.MoveLeft, Vector3.left);

            return direction.sqrMagnitude > 1 ? direction.normalized : direction;
        }
        
        private Vector3 GetDirection(InputAction action, Vector3 direction)
        {
            return Input.GetKey(_inputConfig.GetKeyboardKey(action)) ? direction : Vector3.zero;
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

        public bool IsRunning()
        {
            if (Input.GetKey(_inputConfig.GetKeyboardKey(InputAction.Run)))
                return true;

            return false;
        }

        public bool IsClicked(out Vector3 pos)
        {
            pos = Vector3.zero;
            var button = _inputConfig.GetMouseKey(InputAction.Click);

            if (Input.GetMouseButtonDown((int)button))
            {
                if (Camera.main != null)
                {
                    var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                    
                    var raycastParams = new RaycastParams
                    {
                        Origin = ray.origin,
                        Direction = ray.direction,
                        MaxDistance = 100f,
                        LayerMask = ~0
                    };
                    
                    var filter = new AlwaysTrueFilter();
                    var callback = new PosCallback();
                    
                    _raycaster.Raycast(ref raycastParams, ref filter, ref callback);

                    pos = callback.Pos;
                }
                return true;
            }

            return false;
        }
    }
}