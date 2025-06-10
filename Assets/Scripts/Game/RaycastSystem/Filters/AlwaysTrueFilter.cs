using Core;
using UnityEngine;

namespace Game
{
    public struct AlwaysTrueFilter : IRaycastFilter
    {
        public bool ShouldHit(in RaycastHit hit) => true;
    }
}