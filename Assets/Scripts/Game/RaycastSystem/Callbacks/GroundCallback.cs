using System;
using Core;
using UnityEngine;

namespace Game
{
    public struct GroundCallback : IRaycastCallback
    {
        public Action OnGroundHit;
        private int targetLayer => LayerMask.NameToLayer(Const.GroundLayerName);
        
        public void OnHit(in RaycastHit hit)
        {
            if (hit.collider.gameObject.layer == targetLayer)
            { 
                OnGroundHit?.Invoke();
            }
        }
    }
}