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
        private readonly InputConfig _inputConfig;

        public LookSourceMouse(ILogger logger, InputConfig inputConfig)
        {
            _logger = logger;
            _inputConfig = inputConfig;
        }

        public Vector2 GetLookDelta()
        {
            var x = Input.GetAxis("Mouse X") * _inputConfig.GetSensitivity();
            var y = Input.GetAxis("Mouse Y") * _inputConfig.GetSensitivity();

            var dir = new Vector2(x, y);
            return dir;
        }
    }
}