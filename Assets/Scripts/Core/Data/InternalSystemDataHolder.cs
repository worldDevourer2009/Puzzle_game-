namespace Core
{
    public enum InternalSystemDataType
    {
        PrewardMusicCount,
        PrewardSFXCount,
        MasterMixer,
        MusicMixer,
        UIMixer,
        SFXMixer,
    }
    
    public interface IInternalSystemDataHolder
    {
        T GetSystemParameterByType<T>(InternalSystemDataType type);
    }
    
    public sealed class InternalSystemDataHolder : IInternalSystemDataHolder
    {
        private readonly InternalSettingsConfig _internalSettingsConfig;
        private readonly InternalSettings _internalSettings;

        public InternalSystemDataHolder(InternalSettingsConfig internalSettingsConfig)
        {
            _internalSettingsConfig = internalSettingsConfig;
            _internalSettings = _internalSettingsConfig.InternalSettings;
        }

        public T GetSystemParameterByType<T>(InternalSystemDataType type)
        {
            switch (type)
            {
                case InternalSystemDataType.PrewardMusicCount:
                    if (_internalSettings.PrewardMusicCount is T prewarmCountMusic)
                    {
                        return prewarmCountMusic;
                    }
                    break;
                case InternalSystemDataType.PrewardSFXCount:
                    if (_internalSettings.PrewardSFXCount is T prewarmCountSFX)
                    {
                        return prewarmCountSFX;
                    }
                    break;
                case InternalSystemDataType.MasterMixer:
                    if (_internalSettings.MasterMixer is T masterMixer)
                    {
                        return masterMixer;
                    }
                    break;
                case InternalSystemDataType.MusicMixer:
                    if (_internalSettings.MusicMixer is T musicMixer)
                    {
                        return musicMixer;
                    }
                    break;
                case InternalSystemDataType.UIMixer:
                    if (_internalSettings.UIMixer is T uiMixer)
                    {
                        return uiMixer;
                    }
                    break;
                case InternalSystemDataType.SFXMixer:
                    if (_internalSettings.SFXMixer is T sfxMixer)
                    {
                        return sfxMixer;
                    }
                    break;
                default:
                    break;
            }
            
            return default;
        }
    }
}