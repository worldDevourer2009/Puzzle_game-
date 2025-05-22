using Cysharp.Threading.Tasks;

namespace Core
{
    public interface IAudioPlayer
    {
        UniTask Play();
        UniTask Stop();
    }
    
    public class AudioPlayer
    {
        
    }
}