using UnityEngine;

namespace Core
{
    public interface IRaycastFilter
    {
        bool ShouldHit(in RaycastHit hit);
    }
    
    public struct AlwaysTrueFilter : IRaycastFilter
    {
        public bool ShouldHit(in RaycastHit hit) => true;
    }
}