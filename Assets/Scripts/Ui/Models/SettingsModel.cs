using Core;

namespace Ui
{
    public class SettingsModel
    {
        private readonly IExposedSystemDataHolder _exposedSystemDataHolder;
        
        public SettingsModel(IExposedSystemDataHolder exposedSystemDataHolder)
        {
            _exposedSystemDataHolder = exposedSystemDataHolder;
        }

        public void SetMasterVolume(float value)
        {
            var param = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.MasterVolume);
            param.Value.FloatValue = value;
            param.ForceNotify();
        }

        public void SetMusicVolume(float value)
        {
            var param = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.MusicVolume);
            param.Value.FloatValue = value;
            param.ForceNotify();
        }

        public void SetSFXVolume(float value)
        {
            var param = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.SFXVolume);
            param.Value.FloatValue = value;
            param.ForceNotify();
        }

        public void SetUIVolume(float value)
        {
            var param = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.UIVolume);
            param.Value.FloatValue = value;
            param.ForceNotify();
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