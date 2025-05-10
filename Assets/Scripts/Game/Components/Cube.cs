using UnityEngine;

namespace Game
{
    public class Cube : InteractableComponent
    {
        [SerializeField] private int _index;
        
        public override void Interact()
        {
            Debug.Log($"Interacting with index {_index}");
        }

        public override void StopInteraction()
        {
            throw new System.NotImplementedException();
        }
    }
}