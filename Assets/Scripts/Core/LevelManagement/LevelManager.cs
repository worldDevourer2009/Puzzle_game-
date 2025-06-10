using UnityEngine;

namespace Core
{
    public sealed class LevelManager : MonoBehaviour
    {
        public Transform PlayerDefaultSpawnPoint => _playerDefaultSpawnPoint;
        public Transform InteractableObjectsRoot => _interactableObjectsRoot;
        public Transform ActivatableObjectsRoot => _activatableObjectsRoot;
        
        [SerializeField] private Transform _playerDefaultSpawnPoint;
        [SerializeField] private Transform _interactableObjectsRoot;
        [SerializeField] private Transform _activatableObjectsRoot;
    }
}