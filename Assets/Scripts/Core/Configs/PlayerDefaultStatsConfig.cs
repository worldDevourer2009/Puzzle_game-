using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Player Default Stats", menuName = "Configs/PlayerDefaultStats", order = 2)]
    public class PlayerDefaultStatsConfig : ScriptableObject
    {
        [SerializeField] public PlayerStats playerStats;
    }
}