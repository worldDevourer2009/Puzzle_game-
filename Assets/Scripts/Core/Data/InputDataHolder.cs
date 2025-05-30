using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Core
{
    public interface IInputDataHolder
    {
        ReactiveProperty<KeyCode> GetKeyBinding(InputAction action);
        ReactiveProperty<MouseButton> GetMouseBinding(InputAction action);
        ReactiveProperty<float> Sensitivity { get; }
        ReactiveProperty<float> MinSensitivity { get; }
        ReactiveProperty<float> MaxSensitivity { get; }

        UniTask InitData();
        UniTask LoadData(InputData? savedData = null);
        UniTask<InputData> WriteData();
    }

    public sealed class InputDataHolder : IInputDataHolder
    {
        private readonly InputConfig _config;
        
        public ReactiveProperty<float> Sensitivity => _sensitivity;
        public ReactiveProperty<float> MinSensitivity => _minSensitivity;
        public ReactiveProperty<float> MaxSensitivity => _maxSensitivity;

        private readonly ReactiveProperty<float> _sensitivity;
        private readonly ReactiveProperty<float> _minSensitivity;
        private readonly ReactiveProperty<float> _maxSensitivity;

        private readonly Dictionary<InputAction, ReactiveProperty<KeyCode>> _keyBindings;
        private readonly Dictionary<InputAction, ReactiveProperty<MouseButton>> _mouseBindings;

        public InputDataHolder(InputConfig config)
        {
            _config = config;

            _sensitivity = new ReactiveProperty<float>();
            _minSensitivity = new ReactiveProperty<float>();
            _maxSensitivity = new ReactiveProperty<float>();
            
            _keyBindings = new Dictionary<InputAction, ReactiveProperty<KeyCode>>();
            _mouseBindings = new Dictionary<InputAction, ReactiveProperty<MouseButton>>();
        }

        public UniTask InitData()
        {
            _sensitivity.Value = _config.GetSensitivity();
            _minSensitivity.Value = _config.GetMinSensitivity();
            _maxSensitivity.Value = _config.GetMaxSensitivity();
            
            foreach (InputAction action in Enum.GetValues(typeof(InputAction)))
            {
                if (action == InputAction.Click)
                {
                    var mb = _config.GetMouseKey(action);
                    _mouseBindings[action] = new ReactiveProperty<MouseButton>(mb);
                }
                else
                {
                    var key = _config.GetKeyboardKey(action);
                    _keyBindings[action] = new ReactiveProperty<KeyCode>(key);
                }
            }

            return UniTask.CompletedTask;
        }

        public ReactiveProperty<KeyCode> GetKeyBinding(InputAction action)
        {
            return _keyBindings.GetValueOrDefault(action);
        }

        public ReactiveProperty<MouseButton> GetMouseBinding(InputAction action)
        {
            return _mouseBindings.GetValueOrDefault(action);
        }

        public UniTask LoadData(InputData? savedData = null)
        {
            var data = savedData ?? new InputData
            {
                KeyboardInput = _config.KeyboardInput,
                Sensitivity = _config.GetSensitivity(),
                MaxSensitivity = _config.GetMaxSensitivity(),
                MinSensitivity = _config.GetMinSensitivity()
            };

            foreach (var kvp in _keyBindings)
            {
                var action = kvp.Key;
                var value = data.KeyboardInput.GetType()
                    .GetField(action + "Key")
                    .GetValue(data.KeyboardInput);
                kvp.Value.Value = (KeyCode)value;
            }

            if (_mouseBindings.TryGetValue(InputAction.Click, out var mouseRp))
            {
                mouseRp.Value = data.KeyboardInput.MouseLeft;
            }

            Sensitivity.Value = data.Sensitivity;
            MinSensitivity.Value = data.MinSensitivity;
            MaxSensitivity.Value = data.MaxSensitivity;

            return UniTask.CompletedTask;
        }

        public UniTask<InputData> WriteData()
        {
            var storage = new InputData
            {
                KeyboardInput = new KeyboardInput
                {
                    MoveForwardKey = _keyBindings[InputAction.MoveForward].Value,
                    MoveBackwardKey = _keyBindings[InputAction.MoveBackward].Value,
                    MoveLeftKey = _keyBindings[InputAction.MoveLeft].Value,
                    MoveRightKey = _keyBindings[InputAction.MoveRight].Value,
                    RunKey = _keyBindings[InputAction.Run].Value,
                    JumpKey = _keyBindings[InputAction.Jump].Value,
                    UseKey = _keyBindings[InputAction.Use].Value,
                    PauseKey = _keyBindings[InputAction.Pause].Value,
                    MouseLeft = _mouseBindings[InputAction.Click].Value,
                    MouseRight = _config.KeyboardInput.MouseRight
                },
                Sensitivity = _sensitivity.Value,
                MinSensitivity = _minSensitivity.Value,
                MaxSensitivity = _maxSensitivity.Value
            };

            return UniTask.FromResult(storage);
        }
    }
}