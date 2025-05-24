using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Cube : InteractableLogic
    {
        [SerializeField] private int _index;
        private ILevelManager _levelManager;
        private IPlayerFacade _playerFacade;
        
        [Inject]
        public void Construct(ILevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        public override void Interact()
        {
            Debug.Log($"Interacting with index {_index}");
            var player = _levelManager.PlayerEntity;

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