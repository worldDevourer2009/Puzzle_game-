using System;
using UnityEngine;

namespace Core
{
    public interface IRaycaster
    {
        public void Raycast<TFilter, TCallback>(ref RaycastParams prms, ref TFilter filter, ref TCallback callback
        )
            where TFilter : struct, IRaycastFilter
            where TCallback : struct, IRaycastCallback;
    }

    [Serializable]
    public struct RaycastParams
    {
        public Vector3 Origin;
        public Vector3 Direction;
        public float MaxDistance;
        public LayerMask LayerMask;
    }

    public class RaycasterSystem : IRaycaster
    {
        private readonly RaycastHit[] _hitBuffer;

        public RaycasterSystem()
        {
            _hitBuffer = new RaycastHit[256];
        }

        public void Raycast<TFilter, TCallback>(ref RaycastParams prms, ref TFilter filter, ref TCallback callback) 
            where TFilter : struct, IRaycastFilter 
            where TCallback : struct, IRaycastCallback
        {
            var hitCount = Physics.RaycastNonAlloc(
                prms.Origin, prms.Direction, _hitBuffer, prms.MaxDistance, prms.LayerMask
            );
            
            for (var i = 0; i < hitCount; i++)
            {
                ref var hit = ref _hitBuffer[i];
                
                if (filter.ShouldHit(hit))
                {
                    callback.OnHit(hit);
                }
            }
        }
    }
}