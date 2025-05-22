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

    public struct HitDetectedCallback : IRaycastCallback
    {
        public bool HasHit;
        
        public void OnHit(in RaycastHit hit)
        {
            HasHit = true;
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

    public struct PosCallback : IRaycastCallback
    {
        public Vector3 Pos;

        public void OnHit(in RaycastHit hit)
        {
            Pos = hit.point;
        }
    }
}