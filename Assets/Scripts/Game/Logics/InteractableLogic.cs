using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Game
{
    public abstract class InteractableLogic : MonoBehaviour, IInteractable
    {
        public virtual InteractableType Type => InteractableType.Usable;
        
        private Shader _outlineShader;
        private Shader _outlineShaderMask;
        private Material _outlineMaterial;
        private Material _outlineMaterialMask;
        
        private List<Renderer> _renderers;
        private Dictionary<Renderer, Material[]> _originalMaterials = new ();
        
        private bool _outlineEnabled;
        
        protected virtual void Awake()
        {
            _renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
            
            foreach (var rend in _renderers)
            {
                _originalMaterials[rend] = rend.sharedMaterials;
            }
        }

        protected virtual void Start()
        {
            _outlineShader = Shader.Find("Custom/Outline Fill");
            _outlineShaderMask = Shader.Find("Custom/Outline Mask");
            _outlineMaterial = new Material(_outlineShader);
            _outlineMaterialMask = new Material(_outlineShaderMask);
            _outlineMaterial.hideFlags = HideFlags.HideAndDontSave;
            _outlineMaterialMask.hideFlags = HideFlags.HideAndDontSave;
        }
        
        public abstract void Interact();
        
        public abstract void StopInteraction();
        
        public virtual void Outline()
        {
            if (_outlineEnabled)
            {
                return;
            }
            
            _outlineEnabled = true;
            
            foreach (var rend in _renderers)
            {
                if (rend == null)
                {
                    continue;
                }
                
                var mats = new List<Material>(rend.sharedMaterials);
                mats.Add(_outlineMaterialMask);
                mats.Add(_outlineMaterial);
                rend.materials = mats.ToArray();
            }
        }

        public void ResetOutline()
        {
            if (!_outlineEnabled)
            {
                return;
            }
            
            _outlineEnabled = false;

            if (_renderers == null || _renderers.Count == 0)
            {
                return;
            }
            
            foreach (var rend in _renderers)
            {
                if (rend == null || !rend)
                {
                    continue;
                }
                
                if (_originalMaterials.TryGetValue(rend, out var origMats))
                {
                    if (origMats == null)
                    {
                        continue;
                    }
                    
                    rend.materials = origMats;
                }
            }
        }
        
        protected virtual void OnDestroy()
        {
            _outlineEnabled = false;
        }
    }
}