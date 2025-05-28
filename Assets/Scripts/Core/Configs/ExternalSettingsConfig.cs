using System.Collections.Generic;
using UnityEngine;
using System;

namespace Core
{
    [Serializable]
    public class ExposedSystemParameter
    {
        public ExposedSystemDataType ParameterType; 
        public float FloatValue;
        public int IntValue;
        public bool BoolValue;
    }
    
    [CreateAssetMenu(fileName = "ExternalSettingsConfig", menuName = "Configs/ExternalSettingsConfig", order = 1)]
    public class ExternalSettingsConfig : ScriptableObject
    {
        public List<ExposedSystemParameter> ExposedSystemParameters;
    }
}