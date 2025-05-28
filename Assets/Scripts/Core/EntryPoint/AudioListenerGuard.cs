using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(-100)]
    public class AudioListenerGuard : MonoBehaviour
    {
        private void Awake()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void HandleLog(string condition, string stackTrace, LogType type)
        {
            if (condition.Contains("There are no audio listeners in the scene. Please ensure there is always one audio listener in the scene"))
            {
                return;
            }
            
            
            Debug.unityLogger.Log(type, condition);
        }
    }
}