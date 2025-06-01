using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using ZLinq;

namespace Core
{
    public enum TriggerState
    {
       None,
       Active,
       Inactive,
       Completed
    }

    public interface ITriggerSystem
    {
        void SetTriggers(IEnumerable<string> ids);
        bool IsTriggered(string id);
        void ClearAllTriggers();
        UniTask Trigger(string id);
        UniTask SetTriggerState(string id, TriggerState state);
        IReadOnlyCollection<string> GetActiveTriggers();
        Subject<Unit> GetOrCreateSubject(string id);
    }
    
    public class TriggerSystem : ITriggerSystem
    {
        private readonly ITriggerFactory _triggerFactory;
        private readonly ILogger _logger;
        
        private readonly Dictionary<string, ITrigger> _triggers;
        private readonly Dictionary<string, Subject<Unit>> _subjects;

        public TriggerSystem(ITriggerFactory triggerFactory, ILogger logger)
        {
            _triggerFactory = triggerFactory;
            _logger = logger;

            _triggers = new Dictionary<string, ITrigger>();
            _subjects = new Dictionary<string, Subject<Unit>>();
        }

        public void SetTriggers(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                _logger.LogWarning("Trying to set ids for null list");
                return;
            }

            var idsArray = ids as string[] ?? ids.ToArray();
            
            for (var i = 0; i < idsArray.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(idsArray[i]))
                {
                    continue;
                }

                var trigger = _triggerFactory.CreateTrigger(idsArray[i]);

                if (trigger != null)
                {
                    _triggers[idsArray[i]] = trigger;
                }
            }
        }

        public bool IsTriggered(string id)
        {
            if (_triggers.TryGetValue(id, out var trigger))
            {
                if (trigger != null)
                {
                    if (trigger.state == TriggerState.Completed || trigger.state == TriggerState.Active)
                    {
                        return true;
                    }
                    
                    return false;
                }
            }
            _logger.LogWarning("Trigger was not defiened in the dictionnary return false");
            return false;
        }

        public void ClearAllTriggers()
        {
            _triggers.Clear();
            _subjects.Clear();
        }

        public async UniTask Trigger(string id)
        {
            if (_triggers.TryGetValue(id, out var trigger))
            {
                if (trigger != null)
                {
                    await trigger.Execute();
                }
                else
                {
                    _logger.LogWarning("Trying to apply trigger which is null");
                }
            }
            else
            {
                _logger.LogWarning("Trigger was not defined in the dictionnary");
            }
        }

        public UniTask SetTriggerState(string id, TriggerState state)
        {
            if (_triggers.TryGetValue(id, out var trigger))
            {
                if (trigger != null)
                {
                    trigger.state = state;
                }
            }
            
            return UniTask.CompletedTask;
        }

        public IReadOnlyCollection<string> GetActiveTriggers()
        {
            return _triggers.Values
                .AsValueEnumerable()
                .Where(x => x != null && x.state == TriggerState.Active)
                .Select(x => x.Id)
                .ToList();
        }

        public Subject<Unit> GetOrCreateSubject(string id)
        {
            if (_subjects.TryGetValue(id, out var subject))
            {
                return subject;
            }
            
            subject = new Subject<Unit>();
            _subjects[id] = subject;
            return subject;
        }
    }
}