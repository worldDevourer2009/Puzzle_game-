using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Core
{
    [CreateAssetMenu(fileName = "InternalSettingsConfig", menuName = "Configs/InternalSettingsConfig", order = 1)]
    public class InternalSettingsConfig : ScriptableObject
    {
        public InternalSettings InternalSettings;
    }

    [Serializable]
    public struct InternalSettings
    {
        [Header("Audio System Settings")] 
        public int PrewardMusicCount;
        public int PrewardSFXCount;
        
        [Header("Audio Mixer Groups")]
        public AudioMixerGroup MasterMixer;
        public AudioMixerGroup MusicMixer;
        public AudioMixerGroup UIMixer;
        public AudioMixerGroup SFXMixer;
    }
}