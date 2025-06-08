using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Core
{
    public interface ILocalizationDataHolder
    {
        UniTask InitData();
        Dictionary<LocalizationKey, string> GetLocalizationByLanguage(Language language);
    }
    
    public class LocalizationDataHolder : ILocalizationDataHolder
    {
        private readonly LocalizationConfig _localizationConfig;
        
        public LocalizationDataHolder(LocalizationConfig localizationConfig)
        {
            _localizationConfig = localizationConfig;
        }

        public UniTask InitData()
        {
            return UniTask.CompletedTask;
        }

        public Dictionary<LocalizationKey, string> GetLocalizationByLanguage(Language language)
        {
            return new Dictionary<LocalizationKey, string>();
        }
    }
}