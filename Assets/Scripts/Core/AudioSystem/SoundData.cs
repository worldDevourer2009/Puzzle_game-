using System;
using UnityEngine;

namespace Core
{
    public enum SoundCategory
    {
        None,
        Music,
        UI,
        SFX
    }

    public enum MixerType
    {
        Master,
        Music,
        UI,
        SFX
    }
    
    public enum SoundType
    {
        Sound,
        Music
    }

    [Serializable]
    public class SoundData
    {
        public SoundCategory Category;
        public SoundType SoundType;
        public SoundClipId AudioClipId;
        [Range(0f, 1f)]
        public float Volume = 1f;
        public float Pitch = 1f;
        public bool Loop;
        public bool PlayOnAwake;
        
        public float SpatialBlend;
        public float MinDistance;
        public float MaxDistance;
        public AnimationCurve VolumeRolloff;
        
        public float DopplerLevel;
        
        public bool UseLowPassFilter;
        public float LowPassCutoffFrequency;

        public bool UseHighPassFilter;
        public float HighPassCutoffFrequency;

        public bool UseReverbFilter;
        public float ReverbLevel;
        
        public float Delay;
        
        public float RandomPitchMin = 1f;
        public float RandomPitchMax = 1f;
        
        public float RandomVolumeMin = 1f;
        public float RandomVolumeMax = 1f;

        public int Priority;
        
        public float FadeInDuration;
        public float FadeOutDuration;
    }
}