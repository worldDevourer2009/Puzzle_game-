using Core;
using Zenject;

namespace Game
{
    public class NextLevelObject : InteractableLogic
    {
        private ILevelManager _levelManager;

        [Inject]
        public void Construct(ILevelManager levelManager)
        {
            _levelManager = levelManager;
        }
        
        public override async void Interact()
        {
            var nextLevelId = _levelManager.TryGetNextLevelId();
            await _levelManager.LoadLevelByName(nextLevelId);
        }

        public override void StopInteraction()
        {
            //nope
        }
    }
}