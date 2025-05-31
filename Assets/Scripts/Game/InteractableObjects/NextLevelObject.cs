using Core;
using UnityEngine;
using Zenject;

namespace Game
{
    public class NextLevelObject : InteractableLogic
    {
        [SerializeField] private string _sceneToLoad;
        private ILevelManager _levelManager;

        [Inject]
        public void Construct(ILevelManager levelManager)
        {
            _levelManager = levelManager;
        }
        
        public override async void Interact()
        {
            await _levelManager.LoadLevelByName(_sceneToLoad);
        }

        public override void StopInteraction()
        {
            //nope
        }
    }
}