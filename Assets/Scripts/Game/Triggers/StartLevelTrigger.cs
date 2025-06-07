using Cysharp.Threading.Tasks;

namespace Core
{
    public class StartLevelTrigger : ITrigger
    {
        public string Id => "StartLevel_1";
        public TriggerState state { get; set; } = TriggerState.Inactive;
        private readonly ILevelManager _levelManager;

        public UniTask Execute()
        {
            if (state == TriggerState.Completed)
            {
                return UniTask.CompletedTask;
            }
            
            state = TriggerState.Completed;
            
            return UniTask.CompletedTask;
        }
    }
}