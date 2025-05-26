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

    [Serializable]
    public class SoundData
    {
        public SoundCategory Category;
        public SoundType SoundType;
        public SoundClipId AudioClipId;
        public bool Is3D;
        [Range(0f, 1f)]
        public float Volume = 1f;
        public float Pitch = 1f;
        public bool Loop;
        public bool PlayOnAwake;
        
        public float SpatialBlend;
        public float MinDistance;
        public float MaxDistance;
        public AnimationCurve VolumeRolloff;
        
        public Vector3 PositionOffset;
        public Vector3 Velocity;
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
        
        public bool DestroyOnEnd;
        
        public float FadeInDuration;
        public float FadeOutDuration;
    }
}