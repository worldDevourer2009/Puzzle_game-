using Cysharp.Threading.Tasks;

namespace Core
{
    public class StartLevelTrigger : ITrigger
    {
        public string Id => "StartLevel_1";
        public TriggerState state { get; set; } = TriggerState.Inactive;
        private readonly ISaver _saver;

        private StartLevelTrigger(ISaver saver)
        {
            _saver = saver;
        }

        public async UniTask Execute()
        {
            Logger.Instance.Log("Executirng start level trigger");
            if (state == TriggerState.Completed)
            {
                return;
            }
            
            state = TriggerState.Completed;
            await _saver.SaveAll();
            
            return;
        }
    }
}