using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class SoundEmittable : MonoBehaviour
    {
        public abstract AudioSource AudioSource { get; }
    }
}