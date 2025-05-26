using Core;

namespace Game
{
    public interface IPlayerAudioController
    {
        void PlayJumpSound();
    }
    
    public class PlayerAudioController
    {
        private readonly IPlayerCore _playerCore;
        private readonly IAudioSystem _audioSystem;

        public PlayerAudioController(IPlayerCore playerCore, IAudioSystem audioSystem)
        {
            _playerCore = playerCore;
            _audioSystem = audioSystem;
        }
    }
}