using System;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Player Interaction Config", menuName = "Configs/Player Interaction Config", order = 2)]
    public class PlayerInteractionConfig : ScriptableObject
    {
        [SerializeField] public PlayerInteraction PlayerInteraction;
    }
    
    [Serializable]
    public struct PlayerInteraction
    {
        public LayerMask LayerMask;
        public float InteractionDistance;
    }
}