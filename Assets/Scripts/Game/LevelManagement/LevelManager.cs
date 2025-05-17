using UnityEngine;

namespace Game
{
    public sealed class LevelManager : MonoBehaviour
    {
        public Transform PlayerDefaultSpawnPoint => _playerDefaultSpawnPoint;
        
        [SerializeField] private Transform _playerDefaultSpawnPoint;
        [SerializeField] private Transform b;
        [SerializeField] private Transform c;
    }
}