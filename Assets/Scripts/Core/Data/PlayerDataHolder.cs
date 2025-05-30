using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Core
{
    public enum PlayerDataType
    {
        Speed,
        RunSpeed,
        CrouchSpeed,
        JumpForce,
        GroundableParams,
        LookClamp,
        MaxStepSlopeAngle,
        StepCheckDistance,
        StepMoveDistance,
        StepHeight,
    }

    public enum PlayerInteractionDataType
    {
        LayerMask,
        InteractionDistance,
        LayerMaskStep,
        MoveLayerMask
    }

    public interface IReactiveWrapper
    {
        object GetValue();
    }

    public class ReactiveWrapper<T> : IReactiveWrapper
    {
        private readonly ReactiveProperty<T> _prop;

        public ReactiveWrapper(ReactiveProperty<T> prop)
        {
            _prop = prop;
        }

        public object GetValue()
        {
            return _prop.Value;
        }

        public void SetValue(T value)
        {
            _prop.Value = value;
        }
    }

    public interface IPlayerDataHolder
    {
        public UniTask InitData();
        ReactiveProperty<LayerMask> PlayerInteractionLayerMask { get; }
        ReactiveProperty<LayerMask> PlayerStepInteractionLayerMask { get; }
        ReactiveProperty<LayerMask> PlayerMoveInteractionLayerMask { get; }
        ReactiveProperty<float> PlayerInteractionDistance { get; }
        ReactiveProperty<float> PlayerLookClamp { get; }
        ReactiveProperty<float> PlayerSpeed { get; }
        ReactiveProperty<float> PlayerMaxStepSlopeAngle { get; }
        ReactiveProperty<float> PlayerStepHeight { get; }
        ReactiveProperty<float> PlayerStepCheckDistance { get; }
        ReactiveProperty<float> PlayerStepMoveDistance { get; }
        ReactiveProperty<float> PlayerRunSpeed { get; }
        ReactiveProperty<float> PlayerJumpForce { get; }
        ReactiveProperty<RaycastParams> PlayerGroundableParams { get; }
        ReactiveProperty<float> PlayerCrouchSpeed { get; }
        UniTask ApplySavedData(PlayerStats stats, PlayerInteraction interaction);
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
        public ReactiveProperty<LayerMask> PlayerStepInteractionLayerMask => _playerStepInteractionMask;
        public ReactiveProperty<LayerMask> PlayerMoveInteractionLayerMask => _playerMoveInteractionMask;
        public ReactiveProperty<float> PlayerInteractionDistance => _playerInteractionDistance;
        public ReactiveProperty<float> PlayerLookClamp => _playerLookClamp;

        public ReactiveProperty<float> PlayerSpeed => _playerSpeed;
        public ReactiveProperty<float> PlayerRunSpeed => _playerRunSpeed;
        public ReactiveProperty<float> PlayerJumpForce => _playerJumpForce;
        public ReactiveProperty<RaycastParams> PlayerGroundableParams => _playerGroundableParams;
        public ReactiveProperty<float> PlayerCrouchSpeed => _playerCrouchSpeed;

        public ReactiveProperty<float> PlayerMaxStepSlopeAngle => _playerMaxStepSlopeAngle;
        public ReactiveProperty<float> PlayerStepHeight => _playerStepHeight;
        public ReactiveProperty<float> PlayerStepCheckDistance => _playerStepCheckDistance;
        public ReactiveProperty<float> PlayerStepMoveDistance => _playerStepMoveDistance;

        private readonly ILogger _logger;
        private readonly PlayerInteractionConfig _playerInteractionConfig;
        private readonly PlayerDefaultStatsConfig _defaultStatsConfig;

        private readonly ReactiveProperty<LayerMask> _playerInteractionMask = new();
        private readonly ReactiveProperty<LayerMask> _playerStepInteractionMask = new();
        private readonly ReactiveProperty<LayerMask> _playerMoveInteractionMask = new();
        private readonly ReactiveProperty<float> _playerInteractionDistance = new();

        private readonly ReactiveProperty<float> _playerLookClamp = new();

        private readonly ReactiveProperty<float> _playerSpeed = new();
        private readonly ReactiveProperty<RaycastParams> _playerGroundableParams = new();
        private readonly ReactiveProperty<float> _playerRunSpeed = new();
        private readonly ReactiveProperty<float> _playerJumpForce = new();
        private readonly ReactiveProperty<float> _playerCrouchSpeed = new();

        private readonly ReactiveProperty<float> _playerStepHeight = new();
        private readonly ReactiveProperty<float> _playerMaxStepSlopeAngle = new();
        private readonly ReactiveProperty<float> _playerStepCheckDistance = new();
        private readonly ReactiveProperty<float> _playerStepMoveDistance = new();

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
            _playerLookClamp.Value = playerStats.LookClamp;
            _playerStepCheckDistance.Value = playerStats.StepCheckDistance;
            _playerMaxStepSlopeAngle.Value = playerStats.MaxStepSlopeAngle;
            _playerStepMoveDistance.Value = playerStats.StepMoveDistance;
            _playerStepHeight.Value = playerStats.StepHeight;

            _playerDataStats = new Dictionary<PlayerDataType, IReactiveWrapper>
            {
                { PlayerDataType.Speed, new ReactiveWrapper<float>(_playerSpeed) },
                { PlayerDataType.RunSpeed, new ReactiveWrapper<float>(_playerRunSpeed) },
                { PlayerDataType.CrouchSpeed, new ReactiveWrapper<float>(_playerCrouchSpeed) },
                { PlayerDataType.JumpForce, new ReactiveWrapper<float>(_playerJumpForce) },
                { PlayerDataType.GroundableParams, new ReactiveWrapper<RaycastParams>(_playerGroundableParams) },
                { PlayerDataType.LookClamp, new ReactiveWrapper<float>(_playerLookClamp) },
                { PlayerDataType.MaxStepSlopeAngle, new ReactiveWrapper<float>(_playerMaxStepSlopeAngle) },
                { PlayerDataType.StepCheckDistance, new ReactiveWrapper<float>(_playerStepCheckDistance) },
                { PlayerDataType.StepMoveDistance, new ReactiveWrapper<float>(_playerStepMoveDistance) },
                { PlayerDataType.StepHeight, new ReactiveWrapper<float>(_playerStepHeight) }
            };

            var playerInteractionStats = _playerInteractionConfig.PlayerInteraction;

            _playerInteractionMask.Value = playerInteractionStats.LayerMask;
            _playerInteractionDistance.Value = playerInteractionStats.InteractionDistance;
            _playerStepInteractionMask.Value = playerInteractionStats.StepLayerMask;
            _playerMoveInteractionMask.Value = playerInteractionStats.MoveLayerMask;

            _playerInteractionSetters = new Dictionary<PlayerInteractionDataType, IReactiveWrapper>
            {
                { PlayerInteractionDataType.LayerMask, new ReactiveWrapper<LayerMask>(_playerInteractionMask) },
                {
                    PlayerInteractionDataType.InteractionDistance,
                    new ReactiveWrapper<float>(_playerInteractionDistance)
                },
                { PlayerInteractionDataType.LayerMaskStep, new ReactiveWrapper<LayerMask>(_playerStepInteractionMask) },
                { PlayerInteractionDataType.MoveLayerMask, new ReactiveWrapper<LayerMask>(_playerMoveInteractionMask) },
            };

            return UniTask.CompletedTask;
        }

        public UniTask ApplySavedData(PlayerStats stats, PlayerInteraction interaction)
        {
            _playerSpeed.Value = stats.Speed;
            _playerRunSpeed.Value = stats.RunSpeed;
            _playerCrouchSpeed.Value = stats.CrouchSpeed;
            _playerJumpForce.Value = stats.JumpForce;
            _playerGroundableParams.Value = stats.GroundRaycastParams;

            _playerInteractionDistance.Value = interaction.InteractionDistance;
            _playerInteractionMask.Value = interaction.LayerMask;

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
                _logger.LogWarning($"Wrong type for {dataType}");
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
                            $"Type for {dataType}: expects {wrapper.GetType()}, got {typeof(T)}");
                        break;
                }
            }
            else
            {
                _logger.LogWarning($"No interaction data");
            }
        }

        public T GetPlayerData<T>(PlayerDataType type)
        {
            if (_playerDataStats.TryGetValue(type, out var wrapper)
                && wrapper.GetValue() is T val)
            {
                return val;
            }

            _logger.LogWarning($"Can't get {type}");
            return default;
        }

        public T GetPlayerInteractionData<T>(PlayerInteractionDataType dataType)
        {
            if (_playerInteractionSetters.TryGetValue(dataType, out var wrapper)
                && wrapper.GetValue() is T val)
            {
                return val;
            }

            _logger.LogWarning($"Can't get {dataType}");
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