using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Core
{
    public interface IAudioManager
    {
        UniTask InitAudioManager();
        void SetVolume(MixerType type, float value);
    }

    public class AudioManager : IAudioManager, IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable;
        private readonly IExposedSystemDataHolder _exposedSystemDataHolder;
        private readonly IAudioSystem _audioSystem;

        public AudioManager(IExposedSystemDataHolder exposedSystemDataHolder, IAudioSystem audioSystem)
        {
            _exposedSystemDataHolder = exposedSystemDataHolder;
            _audioSystem = audioSystem;

            _compositeDisposable = new CompositeDisposable();
        }

        public UniTask InitAudioManager()
        {
            var musicVolume = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.MusicVolume);
            var sfxVolume = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.SFXVolume);
            var masterVolume = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.MasterVolume);
            var uiVolume = _exposedSystemDataHolder.GetSystemParameterByType(ExposedSystemDataType.UIVolume);
            
            masterVolume.Subscribe(value => { SetVolume(MixerType.Master, value.FloatValue); })
                .AddTo(_compositeDisposable);

            musicVolume.Subscribe(value => { SetVolume(MixerType.Music, value.FloatValue); })
                .AddTo(_compositeDisposable);

            sfxVolume.Subscribe(value => { SetVolume(MixerType.SFX, value.FloatValue); })
                .AddTo(_compositeDisposable);

            uiVolume.Subscribe(value => { SetVolume(MixerType.UI, value.FloatValue); })
                .AddTo(_compositeDisposable);
            
            return UniTask.CompletedTask;
        }

        public void SetVolume(MixerType type, float value)
        {
            var clampedValue = Mathf.Clamp(value, 0.001f, 1.000001f);
            _audioSystem.SetVolume(type, clampedValue);
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}