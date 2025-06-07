using UnityEngine;

namespace Game
{
    public abstract class ActivatableLogic : MonoBehaviour, IActivatable
    {
        public abstract ActivationState State { get; }
        public abstract void Activate();

        public abstract void Deactivate();
    }
}