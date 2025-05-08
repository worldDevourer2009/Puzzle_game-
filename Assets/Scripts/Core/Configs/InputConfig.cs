using System;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Input Config", menuName = "Configs/Input", order = 1)]
    public class InputConfig : ScriptableObject
    {
        private readonly ILogger _logger;
        [SerializeField] private KeyboardInput _keyboardInput;

        public InputConfig(ILogger logger)
        {
            _logger = logger;
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
                default:
                    _logger.LogError($"Out of range exception in {nameof(GetKeyboardKey)}");
                    break;
            }

            return default;
        }
    }

    [Serializable]
    public struct KeyboardInput
    {
        public KeyCode MoveForwardKey;
        public KeyCode MoveBackwardKey;
        public KeyCode MoveLeftKey;
        public KeyCode MoveRightKey;
        public KeyCode JumpKey;
        public KeyCode UseKey;
    }
}