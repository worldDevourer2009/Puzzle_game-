using System.Collections.Generic;
using R3;

namespace Core
{
    public enum ExposedSystemDataType
    {
        MasterVolume,
        SFXVolume,
        MusicVolume,
        UIVolume,
        Sensitivity,
        Vsync,
    }
    
    public interface IExposedSystemDataHolder
    {
        ReactiveProperty<ExposedSystemParameter> GetSystemParameterByType(ExposedSystemDataType type);
    }
    
    public sealed class ExposedSystemDataHolder : IExposedSystemDataHolder
    {
        private readonly ExternalSettingsConfig _externalSettingsConfig;
        private readonly Dictionary<ExposedSystemDataType, ReactiveProperty<ExposedSystemParameter>> _exposedParameters;

        public ExposedSystemDataHolder(ExternalSettingsConfig externalSettingsConfig)
        {
            _externalSettingsConfig = externalSettingsConfig;
            _exposedParameters = new Dictionary<ExposedSystemDataType, ReactiveProperty<ExposedSystemParameter>>();
            Init();
        }
        
        private void Init()
        {
            var parametersList = _externalSettingsConfig.ExposedSystemParameters;

            if (parametersList == null || parametersList.Count <= 0)
            {
                Logger.Instance.LogWarning("Exposed params are empty or null");
                return;
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
        }

        public ReactiveProperty<ExposedSystemParameter> GetSystemParameterByType(ExposedSystemDataType type)
        {
            return _exposedParameters.GetValueOrDefault(type);
        }
    }
}