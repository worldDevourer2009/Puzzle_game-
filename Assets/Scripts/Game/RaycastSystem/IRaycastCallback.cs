using System;
using Game;
using UnityEngine;

namespace Core
{
    public interface IRaycastCallback
    {
        void OnHit(in RaycastHit hit);
    }
    
    public struct InteractableCallback : IRaycastCallback
    {
        public IInteractable Interactable;
        
        public void OnHit(in RaycastHit hit)
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                Interactable = interactable;
            }
        }
    }

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

    public struct MoveCallBack : IRaycastCallback
    {
        public bool CanMove;
        
        public void OnHit(in RaycastHit hit)
        {
            Debug.Log($"Colliding with {hit.collider.name} move set to false");
            CanMove = false;
        }
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