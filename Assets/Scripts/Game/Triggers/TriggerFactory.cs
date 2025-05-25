using System.Collections.Generic;
using System.Linq;
using ZLinq;

namespace Core
{
    public interface ITriggerFactory
    {
        ITrigger CreateTrigger(string id);
    }
    
    public class TriggerFactory : ITriggerFactory
    {
        private readonly Dictionary<string, ITrigger> _triggers;
        
        public TriggerFactory(IEnumerable<ITrigger> triggers)
        {
            _triggers = new Dictionary<string, ITrigger>();
            var listTriggers = triggers.ToList();
            
            if (listTriggers.AsValueEnumerable().Count() > 0)
            {
                foreach (var trigger in listTriggers)
                {
                    if (trigger == null || string.IsNullOrWhiteSpace(trigger.Id))
                    {
                        continue;
                    }

                    _triggers[trigger.Id] = trigger;
                }
            }
        }

        public ITrigger CreateTrigger(string id)
        {
            return _triggers[id] ?? null;
        }
    }
}