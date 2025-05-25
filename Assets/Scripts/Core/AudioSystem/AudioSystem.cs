using Cysharp.Threading.Tasks;

namespace Core
{
    public enum SoundType
    {
        Sound,
        Music
    }
    
    public interface IAudioSystem
    {
        UniTask CreateSound(string id, SoundType type);
    }
    
    public class AudioSystem
    {
        
    }
}