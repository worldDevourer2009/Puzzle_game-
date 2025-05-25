using UnityEngine;

namespace Core
{
    public class SoundEmitter : SoundEmittable
    {
        public override AudioSource AudioSource
        {
            get
            {
                if (_audioSource == null)
                {
                    _audioSource = GetComponent<AudioSource>();
                }
                
                return _audioSource;
            }
        }

        private AudioSource _audioSource;
    }
}