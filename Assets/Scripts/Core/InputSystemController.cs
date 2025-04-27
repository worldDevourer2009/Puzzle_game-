using R3;

namespace Core
{
    public class InputSystemController : IInputSystemController
    {
        public ISubject<InputSystemControllerAction> InputAction { get; }
    }

    public enum InputSystemControllerAction
    {
        MoveForward,
        MoveBackward,
        MoveUp,
        MoveDown,
        Dash
    }
}