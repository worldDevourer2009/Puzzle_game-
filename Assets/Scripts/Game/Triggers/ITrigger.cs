using Cysharp.Threading.Tasks;

namespace Core
{
    public interface ITrigger
    {
        string Id { get; }
        TriggerState state { get; set; }
        UniTask Execute();
    }
}