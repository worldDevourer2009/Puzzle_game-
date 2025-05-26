using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using ZLinq;
using Object = UnityEngine.Object;

namespace Core
{
    public interface IAudioSystem
    {
        UniTask InitAudioSystem();

        UniTask Play(SoundClipId id);
        UniTask Play(SoundClipId id, Vector3 position);
        UniTask PlayOneShot(SoundClipId id);
        UniTask PlayOneShot(SoundClipId id, Vector3 position);
        UniTask PlayMusic(SoundClipId id);

        void SetSoundPosition(SoundClipId id, Vector3 position);

        void PauseSound(SoundClipId id);
        void PauseAllSounds();
        void ResumeAllSounds();
        void StopSound(SoundClipId id);
        void SetVolume(MixerType category, float volume);
    }

    public class AudioSystem : IAudioSystem
    {
        private readonly IAddressableLoader _addressableLoader;
        private readonly InternalSettingsConfig _internalSettings;
        private readonly AudioDataConfig _audioDataConfig;
        private readonly IPoolSystem _poolSystem;
        private readonly IFactorySystem _factorySystem;
        private readonly Dictionary<SoundClipId, SoundData> _allSounds;
        private readonly Dictionary<SoundClipId, List<SoundEmittable>> _activeEmitters;
        private readonly List<SoundEmittable> _pausedSounds;

        private SoundClipId _currentMusicId;

        public AudioSystem(IAddressableLoader addressableLoader, IPoolSystem poolSystem, IFactorySystem factorySystem,
            InternalSettingsConfig internalSettings, AudioDataConfig audioDataConfig)
        {
            _addressableLoader = addressableLoader;
            _poolSystem = poolSystem;
            _factorySystem = factorySystem;
            _internalSettings = internalSettings;
            _audioDataConfig = audioDataConfig;

            _allSounds = new Dictionary<SoundClipId, SoundData>();
            _activeEmitters = new Dictionary<SoundClipId, List<SoundEmittable>>();
            _pausedSounds = new List<SoundEmittable>();
        }

        public async UniTask InitAudioSystem()
        {
            await _poolSystem.Prewarm<SoundEmittable>(SoundType.Music.ToString(),
                _internalSettings.InternalSettings.PrewardMusicCount, "Music", true);

            await _poolSystem.Prewarm<SoundEmittable>(SoundType.Sound.ToString(),
                _internalSettings.InternalSettings.PrewardSFXCount, "Sounds", true);

            foreach (var data in _audioDataConfig.AudioData)
            {
                if (data != null)
                {
                    _allSounds[data.AudioClipId] = data;
                }
            }
        }

        public async UniTask Play(SoundClipId id)
        {
            await PlayInternal(id, false, null);
        }

        public async UniTask Play(SoundClipId id, Vector3 position)
        {
            await PlayInternal(id, false, position);
        }

        public async UniTask PlayOneShot(SoundClipId id)
        {
            await PlayInternal(id, true, null);
        }

        public async UniTask PlayOneShot(SoundClipId id, Vector3 position)
        {
            await PlayInternal(id, true, position);
        }

        public async UniTask PlayMusic(SoundClipId id)
        {
            if (_currentMusicId != SoundClipId.None)
            {
                StopSound(_currentMusicId);
            }

            _currentMusicId = id;
            await PlayInternal(id, false, null);
        }

        private async UniTask PlayInternal(SoundClipId id, bool oneShot, Vector3? position)
        {
            if (!_allSounds.TryGetValue(id, out var soundData))
            {
                return;
            }

            var (soundEmittable, audioClip) = await CreateSourceAndClip(soundData);

            if (soundEmittable == null || audioClip == null || soundEmittable.AudioSource == null)
            {
                return;
            }

            if (position.HasValue)
            {
                soundEmittable.transform.position = position.Value;
            }

            ConfigureAudioSource(soundEmittable.AudioSource, soundData, audioClip);

            if (soundData.Delay > 0)
            {
                await UniTask.WaitForSeconds(soundData.Delay);
            }

            if (soundData.FadeInDuration > 0f)
            {
                soundEmittable.AudioSource.DOKill();
                soundEmittable.AudioSource.volume = 0f;
                soundEmittable.AudioSource.Play();

                var endVolume = UnityEngine.Random.Range(soundData.RandomVolumeMin, soundData.RandomVolumeMax) *
                                soundData.Volume;

                soundEmittable
                    .AudioSource
                    .DOFade(endVolume, soundData.FadeInDuration).SetEase(Ease.Linear);
            }
            else
            {
                soundEmittable.AudioSource.Play();
            }

            if (oneShot)
            {
                await UniTask.WaitUntil(() => !soundEmittable.AudioSource.isPlaying);
                _factorySystem.Release(soundData.SoundType.ToString(), soundEmittable.gameObject);
            }
            else
            {
                if (!_activeEmitters.TryGetValue(id, out var list))
                {
                    list = new List<SoundEmittable>();
                    _activeEmitters[id] = list;
                }

                list.Add(soundEmittable);
            }
        }

        private async UniTask<(SoundEmittable, AudioClip)> CreateSourceAndClip(SoundData data)
        {
            var obj = await _factorySystem.Create<SoundEmittable>(data.SoundType.ToString());
            var clip = await _addressableLoader.LoadResource<AudioClip>(data.AudioClipId.ToString());
            return (obj, clip);
        }

        private void ConfigureAudioSource(AudioSource source, SoundData data, AudioClip clip)
        {
            source.clip = clip;
            source.loop = data.Loop;
            source.playOnAwake = data.PlayOnAwake;
            source.priority = data.Priority;
            source.volume = data.Volume;

            if (data.Category == SoundCategory.SFX)
            {
                source.spatialBlend = data.SpatialBlend;
                source.minDistance = data.MinDistance;
                source.maxDistance = data.MaxDistance;
                source.dopplerLevel = data.DopplerLevel;

                if (data.VolumeRolloff != null && data.VolumeRolloff.length > 0)
                {
                    source.rolloffMode = AudioRolloffMode.Custom;
                    source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, data.VolumeRolloff);
                }
                else
                {
                    source.rolloffMode = AudioRolloffMode.Logarithmic;
                }
            }

            ApplyFilter<AudioLowPassFilter>(source, data.UseLowPassFilter,
                filter => filter.cutoffFrequency = data.LowPassCutoffFrequency);

            ApplyFilter<AudioHighPassFilter>(source, data.UseHighPassFilter,
                filter => filter.cutoffFrequency = data.HighPassCutoffFrequency);

            ApplyFilter<AudioReverbFilter>(source, data.UseReverbFilter,
                filter => filter.reverbLevel = data.ReverbLevel);

            AddToMixer(source, data);
        }

        private void AddToMixer(AudioSource source, SoundData data)
        {
            var audioSettings = _internalSettings.InternalSettings;
            switch (data.Category)
            {
                case SoundCategory.Music:
                    source.outputAudioMixerGroup = audioSettings.MusicMixer;
                    break;
                case SoundCategory.UI:
                    source.outputAudioMixerGroup = audioSettings.UIMixer;
                    break;
                case SoundCategory.SFX:
                    source.outputAudioMixerGroup = audioSettings.SFXMixer;
                    break;
                case SoundCategory.None:
                default:
                    break;
            }
        }

        private void ApplyFilter<T>(AudioSource src, bool use, Action<T> configure) where T : Behaviour
        {
            var filter = src.GetComponent<T>();
            if (use)
            {
                if (filter == null) filter = src.gameObject.AddComponent<T>();
                configure(filter);
            }
            else if (filter != null)
            {
                Object.Destroy(filter);
            }
        }

        public void SetSoundPosition(SoundClipId id, Vector3 position)
        {
            if (!_activeEmitters.TryGetValue(id, out var list) || list.Count == 0)
            {
                return;
            }

            list.AsValueEnumerable().FirstOrDefault(x => x != null).transform.position = position;
        }

        public void PauseSound(SoundClipId id)
        {
            if (!_activeEmitters.TryGetValue(id, out var list) || list.Count == 0)
            {
                return;
            }

            foreach (var sound in list)
            {
                if (sound == null)
                {
                    continue;
                }

                sound.AudioSource.Pause();
                _pausedSounds.Add(sound);
            }
        }

        public void PauseAllSounds()
        {
            var activeSounds = _activeEmitters;

            foreach (var list in activeSounds.Values)
            {
                foreach (var sound in list)
                {
                    if (sound == null)
                    {
                        continue;
                    }

                    sound.AudioSource.Pause();
                    _pausedSounds.Add(sound);
                }
            }
        }

        public void ResumeAllSounds()
        {
            for (var i = _pausedSounds.Count - 1; i >= 0; i--)
            {
                var sound = _pausedSounds[i];

                if (sound != null)
                {
                    sound.AudioSource?.UnPause();
                    _pausedSounds.RemoveAt(i);
                }
            }
        }

        public void StopSound(SoundClipId id)
        {
            if (!_activeEmitters.TryGetValue(id, out var list) || list.Count == 0)
            {
                return;
            }

            foreach (var sound in list)
            {
                if (sound == null)
                {
                    continue;
                }

                if (sound.AudioSource.isPlaying && _allSounds.TryGetValue(id, out var data) &&
                    data.FadeOutDuration > 0f)
                {
                    sound.AudioSource.DOFade(0f, data.FadeOutDuration).OnComplete(() =>
                    {
                        sound.AudioSource.Stop();
                        sound.AudioSource.DOKill();
                        _factorySystem.Release(_allSounds[id].SoundType.ToString(), sound.gameObject);
                    });
                }
                else
                {
                    sound.AudioSource.Stop();
                    sound.AudioSource.DOKill();
                    _factorySystem.Release(_allSounds[id].SoundType.ToString(), sound.gameObject);
                }
            }

            list.Clear();
        }

        public void SetVolume(MixerType category, float volume)
        {
            var audioSettings = _internalSettings.InternalSettings;
            switch (category)
            {
                case MixerType.Music:
                    audioSettings.MusicMixer.audioMixer.SetFloat("MusicVolume", Mathf.Log(volume) * 20f);
                    break;
                case MixerType.UI:
                    audioSettings.UIMixer.audioMixer.SetFloat("UIVolume", Mathf.Log(volume) * 20f);
                    break;
                case MixerType.SFX:
                    audioSettings.SFXMixer.audioMixer.SetFloat("SFXVolume", Mathf.Log(volume) * 20f);
                    break;
                case MixerType.Master:
                    audioSettings.MasterMixer.audioMixer.SetFloat("MasterVolume", Mathf.Log(volume) * 20f);
                    break;
                default:
                    break;
            }
        }
    }
}