using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ScenesConfig", menuName = "Configs/ScenesConfig", order = 3)]
    public class ScenesConfig : ScriptableObject
    {
        [SerializeField] public List<string> Scenes;
    }
}