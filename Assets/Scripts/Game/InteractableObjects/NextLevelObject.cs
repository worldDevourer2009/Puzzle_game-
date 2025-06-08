using Core;
using Zenject;

namespace Game
{
    public class NextLevelObject : InteractableLogic
    {
        private ILevelManager _levelManager;
        private ISaver _saver;

        [Inject]
        public void Construct(ILevelManager levelManager, ISaver saver)
        {
            _levelManager = levelManager;
            _saver = saver;
        }
        
        public override async void Interact()
        {
            var nextLevelId = _levelManager.TryGetNextLevelId();
            await _levelManager.LoadLevelByName(nextLevelId);
            _saver.SetCurrentLevelIndex(_levelManager.GetCurrentLevelIndex());
            await _saver.SaveAll();
        }

        public override void StopInteraction()
        {
            //nope
        }
    }
}