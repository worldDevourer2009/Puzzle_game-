using System;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Input Config", menuName = "Configs/Input", order = 1)]
    public class InputConfig : ScriptableObject
    {
        public KeyboardInput KeyboardInput => _keyboardInput;
        private ILogger _logger;
        [SerializeField] private KeyboardInput _keyboardInput;

        [SerializeField] private float _sensitivity;

        [SerializeField] private float _maxSensitivity;
        [SerializeField] private float _minSensitivity;

        public float GetSensitivity()
        {
            return _sensitivity;
        }

        public float GetMaxSensitivity()
        {
            return _maxSensitivity;
        }
        
        public float GetMinSensitivity()
        {
            return _minSensitivity;
        }

        public KeyCode GetKeyboardKey(InputAction action)
        {
            switch (action)
            {
                case InputAction.MoveForward:
                    return _keyboardInput.MoveForwardKey;
                case InputAction.MoveBackward:
                    return _keyboardInput.MoveBackwardKey;
                case InputAction.MoveLeft:
                    return _keyboardInput.MoveLeftKey;
                case InputAction.MoveRight:
                    return _keyboardInput.MoveRightKey;
                case InputAction.Jump:
                    return _keyboardInput.JumpKey;
                case InputAction.Use:
                    return _keyboardInput.UseKey;
                case InputAction.Run:
                    return _keyboardInput.RunKey;
                case InputAction.Pause:
                    return _keyboardInput.PauseKey;
                case InputAction.Release:
                    return _keyboardInput.ReleaseKey;
                case InputAction.Click:
                default:
                    _logger.LogError($"Out of range exception in {nameof(GetKeyboardKey)}");
                    break;
            }

            return default;
        }

        public MouseButton GetMouseKey(InputAction action)
        {
            switch (action)
            {
                case InputAction.Click:
                    return _keyboardInput.MouseLeft;
                default:
                    _logger.LogError($"Out of range exception in {nameof(GetKeyboardKey)}");
                    break;
            }

            return default;
        }
    }
    
    [Serializable]
    public struct InputData
    {
        public KeyboardInput KeyboardInput;
        public float Sensitivity;
        public float MaxSensitivity;
        public float MinSensitivity;
    }
    
    [Serializable]
    public struct KeyboardInput
    {
        public KeyCode MoveForwardKey;
        public KeyCode MoveBackwardKey;
        public KeyCode MoveLeftKey;
        public KeyCode MoveRightKey;
        public KeyCode RunKey;
        public KeyCode JumpKey;
        public KeyCode UseKey;
        public MouseButton MouseLeft;
        public MouseButton MouseRight;
        public KeyCode PauseKey;
        public KeyCode ReleaseKey;
    }

    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }
}