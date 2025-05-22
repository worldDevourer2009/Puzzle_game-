using Cysharp.Threading.Tasks;

namespace Core
{
    public interface IAudioSystem
    {
        UniTask CreateSound();
        UniTask CreateMusic();
    }
    
    public class AudioSystem
    {
        
    }
}