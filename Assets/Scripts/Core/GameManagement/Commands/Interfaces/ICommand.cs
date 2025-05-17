using Cysharp.Threading.Tasks;

namespace Core
{
    public interface ICommand
    {
        UniTask ExecuteAsync();
    }
}