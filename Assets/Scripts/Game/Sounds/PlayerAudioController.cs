using System;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public interface IPlayerAudioController
    {
        UniTask PlayPlayerSound(SoundClipId id);
    }
    
    public class PlayerAudioController : IPlayerAudioController, IDisposable
    {
        private readonly IPlayerCore _playerCore;
        private readonly IAudioSystem _audioSystem;
        
        private readonly Action _jumpHolder;
        private readonly Action<Vector3, bool> _moveHolder;
        private readonly Action _useHolder;
        
        private float _lastStepTime;
        private const float StepCooldown = 0.4f;

        public PlayerAudioController(IPlayerCore playerCore, IAudioSystem audioSystem)
        {
            _playerCore = playerCore;
            _audioSystem = audioSystem;
            
            _jumpHolder = async () => await PlayPlayerSound(SoundClipId.JumpSoundPlayer);
            _moveHolder = async (x, y) => await HandleStep(x, y);
            _useHolder = async () => await PlayPlayerSound(SoundClipId.UseSoundPlayer);

            _playerCore.OnJump += _jumpHolder;
            _playerCore.OnMove += _moveHolder;
            _playerCore.OnUse += _useHolder;
        }
        
        public async UniTask PlayPlayerSound(SoundClipId id)
        {
            await _audioSystem.PlayOneShot(id, _playerCore.GetPlayer().CenterBottomTransform.position);
        }
        
        private async UniTask HandleStep(Vector3 position, bool isGrounded)
        {
            var currentTime = Time.time;
            
            if (currentTime - _lastStepTime < StepCooldown)
            {
                return;
            }

            _lastStepTime = currentTime;

            await _audioSystem.PlayOneShot(SoundClipId.StepSoundPlayer, _playerCore.GetPlayer().CenterBottomTransform.position);
        }

        public void Dispose()
        {
            _playerCore.OnJump -= _jumpHolder;
            _playerCore.OnMove -= _moveHolder;
            _playerCore.OnUse -= _useHolder;
        }
    }
}