using Core;

namespace Ui
{
    public class SettingsModel
    {
        private readonly IExposedSystemDataHolder _exposedSystemDataHolder;
        private readonly IInputDataHolder _inputDataHolder;
        private readonly ISaver _saver;
        
        public SettingsModel(IExposedSystemDataHolder exposedSystemDataHolder, IInputDataHolder inputDataHolder, ISaver saver)
        {
            _exposedSystemDataHolder = exposedSystemDataHolder;
            _inputDataHolder = inputDataHolder;
            _saver = saver;
        }

        public void SetMasterVolume(float value)
        {
            var param = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.MasterVolume);
            param.Value.FloatValue = value;
            param.ForceNotify();
            _saver.SaveSettings();
        }

        public void SetMusicVolume(float value)
        {
            var param = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.MusicVolume);
            param.Value.FloatValue = value;
            param.ForceNotify();
            _saver.SaveSettings();
        }

        public void SetSFXVolume(float value)
        {
            var param = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.SFXVolume);
            param.Value.FloatValue = value;
            param.ForceNotify();
            _saver.SaveSettings();
        }

        public void SetUIVolume(float value)
        {
            var param = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.UIVolume);
            param.Value.FloatValue = value;
            param.ForceNotify();
            _saver.SaveSettings();
        }

        public void SetSensitivity(float value)
        {
            if (_inputDataHolder.MinSensitivity.Value > value)
            {
                return;
            }
            
            _inputDataHolder.Sensitivity.Value = value;
            _saver.SaveSettings();
        }

        public float GetMaxSensitivity()
        {
            return _inputDataHolder.MaxSensitivity.Value;
        }
        
        public float GetCurrentSensitivity()
        {
            return _inputDataHolder.Sensitivity.Value;
        }

        public float GetMinSensitivity()
        {
            return _inputDataHolder.MinSensitivity.Value;
        }

        public float GetCurrentMasterVolume()
        {
            var param = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.MasterVolume);
            return param.Value.FloatValue;
        }
        
        public float GetCurrentMusicVolume()
        {
            var param = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.MusicVolume);
            return param.Value.FloatValue;
        }
        
        public float GetCurrentSFXVolume()
        {
            var param = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.SFXVolume);
            return param.Value.FloatValue;
        }
        
        public float GetCurrentUIVolume()
        {
            var param = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.UIVolume);
            return param.Value.FloatValue;
        }
    }
}