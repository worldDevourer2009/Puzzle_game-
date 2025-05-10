using UnityEngine;

namespace Core
{
    public interface IInputSource
    {
        Vector3 GetMoveDirection();
        bool IsJumpPressed();
        bool IsUsePressed();
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