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
        public bool Is3D = true;
        public float Volume = 1f;
        public float Pitch = 1f;
        public bool Loop = false;
        public bool PlayOnAwake;
        
        public float SpatialBlend = 1f;
        public float MinDistance = 1f;
        public float MaxDistance = 500f;
        public AnimationCurve VolumeRolloff;
        
        public Vector3 PositionOffset;
        public Vector3 Velocity;
        public float DopplerLevel = 1f;
        
        public bool UseLowPassFilter = false;
        public float LowPassCutoffFrequency = 5000f;

        public bool UseHighPassFilter = false;
        public float HighPassCutoffFrequency = 500f;

        public bool UseReverbFilter = false;
        public float ReverbLevel = 0f;
        
        public float Delay = 0f;
        public float RandomPitchMin = 1f;
        public float RandomPitchMax = 1f;
        public float RandomVolumeMin = 1f;
        public float RandomVolumeMax = 1f;

        public int Priority = 128;
        
        public bool DestroyOnEnd = true;
        
        public float FadeInDuration = 0f;
        public float FadeOutDuration = 0f;
    }
}