using Core;
using UnityEngine;

namespace Game
{
    public struct NormalCaptureCallback : IRaycastCallback
    {
        public bool HasHit;
        public Vector3 HitNormal;

        public void OnHit(in RaycastHit hit)
        {
            HasHit = true;
            HitNormal = hit.normal;
        }
    }
}