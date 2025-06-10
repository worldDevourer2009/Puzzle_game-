using Core;
using UnityEngine;

namespace Game
{
    public struct MoveCallBack : IRaycastCallback
    {
        public bool CanMove;

        public void OnHit(in RaycastHit hit)
        {
            CanMove = false;
        }
    }
}