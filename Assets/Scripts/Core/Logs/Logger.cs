using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public interface ILogger
    {
        void Log(string log);
        void LogWarning(string log);
        void LogError(string log);
        HashSet<string> GetLogs();
        void ClearLogs();
    }

    public sealed class Logger : ILogger
    {
        public static Logger Instance => _instance ??= new Logger();
        private static Logger _instance;
        
        private readonly HashSet<string> _logs;

        public Logger()
        {
            _logs = new HashSet<string>();
        }

        public void Log(string log)
        {
            Debug.Log(log);

            if (_logs.Contains(log))
                return;

            _logs.Add(log);
        }

        public void LogWarning(string log)
        {
            Debug.Log(log);

            if (_logs.Contains(log))
                return;

            _logs.Add(log);
        }

        public void LogError(string log)
        {
            Debug.LogError(log);

            if (_logs.Contains(log))
                return;

            _logs.Add(log);
        }

        public HashSet<string> GetLogs() =>
            _logs ?? new HashSet<string>();

        public void ClearLogs() => 
            _logs.Clear();
    }
}