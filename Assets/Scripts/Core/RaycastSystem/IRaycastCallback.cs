using UnityEngine;

namespace Core
{
    public interface IRaycastCallback
    {
        void OnHit(in RaycastHit hit);
    }
    
    public struct PosCallback : IRaycastCallback
    {
        public Vector3 Pos;

        public void OnHit(in RaycastHit hit)
        {
            Pos = hit.point;
        }
    }
}