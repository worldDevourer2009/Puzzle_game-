using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Core
{
    public interface ILocalizationDataHolder
    {
        UniTask InitData();
        Dictionary<LocalizationKey, string> GetLocalizationByLanguage(Language language);
    }
    
    public class LocalizationDataHolder
    {
        private readonly LocalizationConfig _localizationConfig;
        
        public LocalizationDataHolder(LocalizationConfig localizationConfig)
        {
            _localizationConfig = localizationConfig;
        }
    }
}