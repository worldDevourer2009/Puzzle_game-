using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public struct LevelData
    {
        public int LevelIndex;
        public int LastTriggeredEventIndex;
    }
}