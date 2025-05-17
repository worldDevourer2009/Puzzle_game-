using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Core
{
    public interface ICommandInvoker
    {
        UniTask ExecuteCommandAsync(ICommand command);
    }
    
    public class CommandInvoker : ICommandInvoker
    {
        private readonly Stack<ICommand> _history;

        public CommandInvoker()
        {
            _history = new Stack<ICommand>();
        }

        public async UniTask ExecuteCommandAsync(ICommand command)
        {
            await command.ExecuteAsync();
            _history.Push(command);
        }
    }
}