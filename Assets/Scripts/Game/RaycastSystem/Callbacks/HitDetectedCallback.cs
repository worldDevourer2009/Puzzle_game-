using Core;
using UnityEngine;

namespace Game
{
    public struct HitDetectedCallback : IRaycastCallback
    {
        public bool HasHit;
        public RaycastHit Hit;
        
        public void OnHit(in RaycastHit hit)
        {
            HasHit = true;
            Hit = hit;
        }
    }
}