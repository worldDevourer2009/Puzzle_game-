using R3;

namespace Core
{
    public interface IInputSystemController
    {
        ISubject<InputSystemControllerAction> InputAction { get; }
    }
}