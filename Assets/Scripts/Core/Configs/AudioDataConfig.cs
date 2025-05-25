using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "AudioDataConfig", menuName = "Configs/AudioDataConfig")]
    public class AudioDataConfig : ScriptableObject
    {
        [SerializeField] private List<SoundData> audioData;
        [SerializeField] private int maxSounds = 50;
        public IReadOnlyList<SoundData> AudioData => audioData;
        public int MaxSound => maxSounds;
    }
}