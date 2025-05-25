using Cysharp.Threading.Tasks;

namespace Core
{
    public interface IState
    {
        GameState Name { get; }
        
        UniTask OnEnter();
        UniTask OnUpdate();
        UniTask OnExit();

        GameState? NextState(GameTrigger trigger);
    }
}