using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using ZLinq;

namespace Core
{
    public enum ExposedSystemDataType
    {
        MasterVolume,
        SFXVolume,
        MusicVolume,
        UIVolume,
        Sensitivity,
        MaxSensitivity,
        MinSensitivity,
        Vsync,
    }

    public interface IExposedSystemDataHolder
    {
        ReactiveProperty<ExposedSystemParameter> GetSystemParameterByType(ExposedSystemDataType type);
        UniTask InitData();
        UniTask LoadData(List<ExposedSystemParameter> savedParams);
        UniTask<List<ExposedSystemParameter>> WriteData();
    }

    public sealed class ExposedSystemDataHolder : IExposedSystemDataHolder
    {
        private readonly ExternalSettingsConfig _externalSettingsConfig;
        private readonly Dictionary<ExposedSystemDataType, ReactiveProperty<ExposedSystemParameter>> _exposedParameters;

        public ExposedSystemDataHolder(ExternalSettingsConfig externalSettingsConfig)
        {
            _externalSettingsConfig = externalSettingsConfig;
            _exposedParameters = new Dictionary<ExposedSystemDataType, ReactiveProperty<ExposedSystemParameter>>();
        }

        public UniTask InitData()
        {
            var parametersList = _externalSettingsConfig.ExposedSystemParameters;

            if (parametersList == null || parametersList.Count <= 0)
            {
                Logger.Instance.LogWarning("Exposed params are empty or null");
                return UniTask.CompletedTask;
            }

            for (var i = 0; i < parametersList.Count; i++)
            {
                if (parametersList[i] == null)
                {
                    continue;
                }

                var reactiveProperty = new ReactiveProperty<ExposedSystemParameter>();
                reactiveProperty.Value = parametersList[i];
                _exposedParameters[parametersList[i].ParameterType] = reactiveProperty;
            }
            
            return UniTask.CompletedTask;
        }

        public ReactiveProperty<ExposedSystemParameter> GetSystemParameterByType(ExposedSystemDataType type)
        {
            return _exposedParameters.GetValueOrDefault(type);
        }

        public UniTask LoadData(List<ExposedSystemParameter> savedParams)
        {
            var source = savedParams ?? _externalSettingsConfig.ExposedSystemParameters;
            if (source != null)
            {
                foreach (var param in source)
                {
                    if (param == null) continue;
                    _exposedParameters[param.ParameterType] = new ReactiveProperty<ExposedSystemParameter>(param);
                }
            }
            else
            {
                foreach (ExposedSystemDataType type in Enum.GetValues(typeof(ExposedSystemDataType)))
                {
                    if (!_exposedParameters.ContainsKey(type))
                        _exposedParameters[type] = new ReactiveProperty<ExposedSystemParameter>(
                            new ExposedSystemParameter { ParameterType = type }
                        );
                }
            }
            
            return UniTask.CompletedTask;
        }

        public UniTask<List<ExposedSystemParameter>> WriteData()
        {
            foreach (ExposedSystemDataType type in Enum.GetValues(typeof(ExposedSystemDataType)))
            {
                if (!_exposedParameters.ContainsKey(type))
                {
                    var defaultParam = _externalSettingsConfig.ExposedSystemParameters?
                                           .AsValueEnumerable()
                                           .FirstOrDefault(p => p.ParameterType == type)
                                       ?? new ExposedSystemParameter { ParameterType = type };

                    var rp = new ReactiveProperty<ExposedSystemParameter>(defaultParam);
                    _exposedParameters[type] = rp;
                }
            }

            var list = _exposedParameters
                .AsValueEnumerable()
                .OrderBy(kv => kv.Key)
                .Select(kv => kv.Value.Value)
                .ToList();

            _externalSettingsConfig.ExposedSystemParameters = list;

            return UniTask.FromResult(list);
        }
    }
}