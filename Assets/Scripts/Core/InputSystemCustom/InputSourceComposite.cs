using System.Collections.Generic;
using UnityEngine;
using ZLinq;

namespace Core
{
    public interface IInputSourceComposite
    {
        Vector3 GetMoveDirection();
        bool Jumped();
        bool Pressed();
        bool IsRunning();
        bool Released();
        bool Paused();
        bool Clicked(out Vector3 pos);
    }

    public class InputSourceComposite : IInputSourceComposite
    {
        private HashSet<IInputSource> _inputSources;

        public InputSourceComposite(IEnumerable<IInputSource> inputSources)
        {
            _inputSources = new HashSet<IInputSource>();
            var inputSourcesExternal = inputSources.AsValueEnumerable().ToList();

            foreach (var input in inputSourcesExternal)
            {
                if (!_inputSources.Add(input))
                {
                    continue;
                }
            }
        }

        public Vector3 GetMoveDirection()
        {
            foreach (var input in _inputSources)
            {
                if (input == null)
                {
                    continue;
                }

                var moveDir = input.GetMoveDirection();
                
                if (moveDir != Vector3.zero)
                {
                    return input.GetMoveDirection();
                }
            }

            return default;
        }

        public bool Jumped()
        {
            foreach (var input in _inputSources)
            {
                if (input == null)
                {
                    continue;
                }

                if (input.Jumped())
                {
                    return true;
                }
            }

            return false;
        }

        public bool Pressed()
        {
            foreach (var input in _inputSources)
            {
                if (input == null)
                {
                    continue;
                }
                
                if (input.Pressed())
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsRunning()
        {
            foreach (var input in _inputSources)
            {
                if (input == null)
                {
                    continue;
                }

                if (input.IsRunning())
                {
                    return true;
                }
            }

            return false;
        }

        public bool Released()
        {
            foreach (var input in _inputSources)
            {
                if (input == null)
                {
                    continue;
                }
                
                if (input.Released())
                {
                    return true;
                }
            }

            return false;
        }

        public bool Paused()
        {
            foreach (var input in _inputSources)
            {
                if (input == null)
                {
                    continue;
                }

                if (input.Paused())
                    return true; 
            }

            return false;
        }

        public bool Clicked(out Vector3 pos)
        {
            pos = Vector3.zero;

            foreach (var input in _inputSources)
            {
                if (input == null)
                {
                    continue;
                }

                if (input.Clicked(out var p))
                {
                    pos = p;
                    return true;
                }
            }

            return false;
        }
    }
}