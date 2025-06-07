using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Core
{
    public enum Language
    {
        English,
        Russian,
        French
    }

    public enum LocalizationKey
    {
        MainMenu_Start,
        MainMenu_Exit,
        MainMenu_Settings,

        Gameplay_Interact,
        Gameplay_Pause,
        Gameplay_Resume,

        Settings_Language,
        Settings_Audio,
        Settings_Graphics,
    }

    public class Localizer
    {
        public static Localizer Instance { get; private set; }

        private readonly ILocalizationDataHolder _localizationDataHolder;
        private readonly Dictionary<Language, Dictionary<LocalizationKey, string>> _localizations;
        private Language _currentLanguage = Language.English;

        private Localizer(ILocalizationDataHolder localizationDataHolder)
        {
            _localizationDataHolder = localizationDataHolder;
            _localizations = new Dictionary<Language, Dictionary<LocalizationKey, string>>();
            Instance = this;
        }

        public async UniTask InitLocalizations()
        {
            await _localizationDataHolder.InitData();

            foreach (Language language in Enum.GetValues(typeof(Language)))
            {
                var localizationDict = _localizationDataHolder.GetLocalizationByLanguage(language);
                
                if (localizationDict.Count > 0)
                {
                    _localizations[language] = localizationDict;
                }
            }
        }

        public string Localize(LocalizationKey localizationKey)
        {
            if (_localizations.TryGetValue(_currentLanguage, out var currentLanguageDict) &&
                currentLanguageDict.TryGetValue(localizationKey, out var text))
            {
                return text;
            }
            
            if (_currentLanguage != Language.English &&
                _localizations.TryGetValue(Language.English, out var englishDict) &&
                englishDict.TryGetValue(localizationKey, out var fallbackText))
            {
                return fallbackText;
            }
            
            return $"[{localizationKey}]";
        }

        public void SetLanguage(Language language)
        {
            _currentLanguage = language;
        }

        public Language GetCurrentLanguage()
        {
            return _currentLanguage;
        }
    }
}