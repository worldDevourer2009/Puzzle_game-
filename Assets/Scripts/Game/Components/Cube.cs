using UnityEngine;
using Zenject;

namespace Game
{
    public class Cube : InteractableComponent
    {
        [SerializeField] private int _index;
        private IPlayerCore _playerCore;
        
        [Inject]
        public void Construct(IPlayerCore playerCore)
        {
            _playerCore = playerCore;
        }
        
        public override void Interact()
        {
            Debug.Log($"Interacting with index {_index}");
            var player = _playerCore.GetPlayer();

            if (player == null)
            {
                Debug.Log("Player is null");
                return;
            }

            var rightHand = player.RightHandTransform;

            if (rightHand == null)
            {
                Debug.Log("Right hand is null");
                return;
            }

            gameObject.transform.SetParent(rightHand);
            gameObject.transform.position = rightHand.position;
        }

        public override void StopInteraction()
        {
            throw new System.NotImplementedException();
        }
    }
}