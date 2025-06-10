using Core;
using UnityEngine;

namespace Game
{
    public struct PosCallback : IRaycastCallback
    {
        public Vector3 Pos;

        public void OnHit(in RaycastHit hit)
        {
            Pos = hit.point;
        }
    }
}