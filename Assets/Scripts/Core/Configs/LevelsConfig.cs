using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Levels Config Config", menuName = "Configs/Levels Config", order = 3)]
    public class LevelsConfig : ScriptableObject
    {
        [SerializeField] public List<LevelData> LevelData;
    }
}