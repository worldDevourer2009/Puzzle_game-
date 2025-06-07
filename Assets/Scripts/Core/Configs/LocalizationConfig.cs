using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "LocalizationConfig", menuName = "Configs/LocalizationConfig", order = 5)]
    public class LocalizationConfig : ScriptableObject
    {
        [SerializeField] private List<Localizaton> _localizations;
        public IReadOnlyList<Localizaton> LocalizationsList => _localizations;
    }

    [Serializable]
    public class Localizaton
    {
        public Language Language;
        public List<LocalizationEntry> Texts;
    }
    
    [Serializable]
    public class LocalizationEntry
    {
        public LocalizationKey Key;
        public string Text;
    }
}