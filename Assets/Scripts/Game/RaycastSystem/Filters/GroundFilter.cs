using Core;
using UnityEngine;

namespace Game
{
    public struct GroundFilter : IRaycastFilter
    {
        private int targetLayer => LayerMask.NameToLayer(Const.GroundLayerName);

        public bool ShouldHit(in RaycastHit hit)
        {
            return hit.collider.gameObject.layer == targetLayer;
        }
    }
}