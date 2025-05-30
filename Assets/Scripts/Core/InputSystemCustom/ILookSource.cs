using UnityEngine;

namespace Core
{
    public interface ILookSource
    {
        Vector2 GetLookDelta();
    }

    public sealed class LookSourceMouse : ILookSource
    {
        private readonly ILogger _logger;
        private readonly IInputDataHolder _inputDataHolder;

        public LookSourceMouse(ILogger logger, IInputDataHolder inputDataHolder)
        {
            _logger = logger;
            _inputDataHolder = inputDataHolder;
        }

        public Vector2 GetLookDelta()
        {
            var x = Input.GetAxis("Mouse X") * _inputDataHolder.Sensitivity.Value;
            var y = Input.GetAxis("Mouse Y") * _inputDataHolder.Sensitivity.Value;

            var dir = new Vector2(x, y);
            return dir;
        }
    }
}