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

    public struct MoveCallBack : IRaycastCallback
    {
        public bool CanMove;

        public void OnHit(in RaycastHit hit)
        {
            CanMove = false;
        }
    }

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

    public struct GroundCallback : IRaycastCallback
    {
        public Action OnGroundHit;
        private int targetLayer => LayerMask.NameToLayer(Const.GroundLayerName);
        
        public void OnHit(in RaycastHit hit)
        {
            if (hit.collider.gameObject.layer == targetLayer)
            { 
                Debug.Log("Setting to true");
                OnGroundHit?.Invoke();
            }
            else
            {
                Debug.Log("Setting to false");
            }
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