using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using ILogger = Core.ILogger;

namespace Game
{
    public enum PlayerDataType
    {
        Speed,
        RunSpeed,
        CrouchSpeed,
        JumpForce,
        GroundableParams,
    }

    public enum PlayerInteractionDataType
    {
        LayerMask,
        InteractionDistance
    }

    public interface IReactiveWrapper
    {
        object GetValue();
    }

    public class ReactiveWrapper<T> : IReactiveWrapper
    {
        private readonly ReactiveProperty<T> _prop;
        public ReactiveWrapper(ReactiveProperty<T> prop) => _prop = prop;
        public object GetValue() => _prop.Value;
        public void SetValue(T value) => _prop.Value = value;
    }

    public interface IPlayerDataHolder
    {
        public UniTask InitData();
        ReactiveProperty<LayerMask> PlayerInteractionLayerMask { get; }
        ReactiveProperty<float> PlayerInteractionDistance { get; }
        ReactiveProperty<float> PlayerSpeed { get; }
        ReactiveProperty<float> PlayerRunSpeed { get; }
        ReactiveProperty<float> PlayerJumpForce { get; }
        ReactiveProperty<RaycastParams> PlayerGroundableParams { get; }
        ReactiveProperty<float> PlayerCrouchSpeed { get; }
        void SetPlayerData(PlayerDataType dataType, float value);
        void SetInteractionData<T>(PlayerInteractionDataType key, T value);
        T GetPlayerData<T>(PlayerDataType dataType);
        T GetPlayerInteractionData<T>(PlayerInteractionDataType dataType);
        UniTask<(PlayerStats, PlayerInteraction)> WriteData();
    }

    public class PlayerDataHolder : IPlayerDataHolder, IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable = new();

        public ReactiveProperty<LayerMask> PlayerInteractionLayerMask => _playerInteractionMask;
        public ReactiveProperty<float> PlayerInteractionDistance => _playerInteractionDistance;

        public ReactiveProperty<float> PlayerSpeed => _playerSpeed;
        public ReactiveProperty<float> PlayerRunSpeed => _playerRunSpeed;
        public ReactiveProperty<float> PlayerJumpForce => _playerJumpForce;
        public ReactiveProperty<RaycastParams> PlayerGroundableParams => _playerGroundableParams;
        public ReactiveProperty<float> PlayerCrouchSpeed => _playerCrouchSpeed;

        private readonly ILogger _logger;
        private readonly PlayerInteractionConfig _playerInteractionConfig;
        private readonly PlayerDefaultStatsConfig _defaultStatsConfig;

        private readonly ReactiveProperty<LayerMask> _playerInteractionMask = new();
        private readonly ReactiveProperty<float> _playerInteractionDistance = new();

        private readonly ReactiveProperty<float> _playerSpeed = new();
        private readonly ReactiveProperty<RaycastParams> _playerGroundableParams = new();
        private readonly ReactiveProperty<float> _playerRunSpeed = new();
        private readonly ReactiveProperty<float> _playerJumpForce = new();
        private readonly ReactiveProperty<float> _playerCrouchSpeed = new();

        private Dictionary<PlayerDataType, IReactiveWrapper> _playerDataStats;
        private Dictionary<PlayerInteractionDataType, IReactiveWrapper> _playerInteractionSetters;

        public PlayerDataHolder(ILogger logger, PlayerInteractionConfig playerInteractionConfig,
            PlayerDefaultStatsConfig defaultStatsConfig)
        {
            _logger = logger;
            _playerInteractionConfig = playerInteractionConfig;
            _defaultStatsConfig = defaultStatsConfig;
        }

        public UniTask InitData()
        {
            var playerStats = _defaultStatsConfig.playerStats;
            _playerSpeed.Value = playerStats.Speed;
            _playerRunSpeed.Value = playerStats.RunSpeed;
            _playerCrouchSpeed.Value = playerStats.CrouchSpeed;
            _playerJumpForce.Value = playerStats.JumpForce;
            _playerGroundableParams.Value = playerStats.GroundRaycastParams;

            _playerDataStats = new Dictionary<PlayerDataType, IReactiveWrapper>
            {
                { PlayerDataType.Speed, new ReactiveWrapper<float>(_playerSpeed) },
                { PlayerDataType.RunSpeed, new ReactiveWrapper<float>(_playerRunSpeed) },
                { PlayerDataType.CrouchSpeed, new ReactiveWrapper<float>(_playerCrouchSpeed) },
                { PlayerDataType.JumpForce, new ReactiveWrapper<float>(_playerJumpForce) },
                { PlayerDataType.GroundableParams, new ReactiveWrapper<RaycastParams>(_playerGroundableParams) },
            };

            var playerInteractionStats = _playerInteractionConfig.PlayerInteraction;

            _playerInteractionMask.Value = playerInteractionStats.LayerMask;
            _playerInteractionDistance.Value = playerInteractionStats.InteractionDistance;

            _playerInteractionSetters = new Dictionary<PlayerInteractionDataType, IReactiveWrapper>
            {
                { PlayerInteractionDataType.LayerMask, new ReactiveWrapper<LayerMask>(_playerInteractionMask) },
                {
                    PlayerInteractionDataType.InteractionDistance,
                    new ReactiveWrapper<float>(_playerInteractionDistance)
                }
            };

            return UniTask.CompletedTask;
        }

        public void SetPlayerData(PlayerDataType dataType, float value)
        {
            if (_playerDataStats.TryGetValue(dataType, out var wrapper)
                && wrapper is ReactiveWrapper<float> floatWrapper)
            {
                floatWrapper.SetValue(value);
            }
            else
            {
                _logger.LogWarning($"Wrong type or missing entry for {dataType}");
            }
        }

        public void SetInteractionData<T>(PlayerInteractionDataType dataType, T value)
        {
            if (_playerInteractionSetters.TryGetValue(dataType, out var wrapper))
            {
                switch (wrapper)
                {
                    case ReactiveWrapper<T> typedWrapper:
                        typedWrapper.SetValue(value);
                        break;
                    default:
                        _logger.LogWarning(
                            $"Type mismatch for {dataType}: expected {wrapper.GetType()}, got {typeof(T)}");
                        break;
                }
            }
            else
            {
                _logger.LogWarning($"No interaction data entry for {dataType}");
            }
        }

        public T GetPlayerData<T>(PlayerDataType type)
        {
            if (_playerDataStats.TryGetValue(type, out var wrapper)
                && wrapper.GetValue() is T val)
            {
                return val;
            }

            _logger.LogWarning($"Can't get {type} as {typeof(T).Name}");
            return default;
        }

        public T GetPlayerInteractionData<T>(PlayerInteractionDataType dataType)
        {
            if (_playerInteractionSetters.TryGetValue(dataType, out var wrapper)
                && wrapper.GetValue() is T val)
            {
                return val;
            }

            _logger.LogWarning($"Can't get {dataType} as {typeof(T).Name}");
            return default;
        }

        public UniTask<(PlayerStats, PlayerInteraction)> WriteData()
        {
            var playerStats = new PlayerStats
            {
                Speed = _playerSpeed.Value,
                CrouchSpeed = _playerCrouchSpeed.Value,
                GroundRaycastParams = _playerGroundableParams.Value,
                JumpForce = _playerJumpForce.Value,
                RunSpeed = _playerRunSpeed.Value
            };

            var playerInteractionStats = new PlayerInteraction
            {
                InteractionDistance = _playerInteractionDistance.Value,
                LayerMask = _playerInteractionMask.Value
            };

            return UniTask.FromResult((playerStats, playerInteractionStats));
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}