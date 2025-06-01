using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [Serializable]
    public struct LevelData
    {
        public string LevelName;
        public string LastTriggeredEventName;
        public Vector3 PlayerSpawn;
        public List<EntityConfig> Entities;
        public List<TriggerConfig> Triggers;
    }

    [Serializable]
    public class EntityConfig
    {
        public string EntityId;
        public Vector3 Position;
        public Vector3 Rotation;
        public string ParentId;
    }

    [Serializable]
    public class TriggerConfig
    {
        public string TriggerId;
        public TriggerState InitialState;
        public string DependsOn;
    }
}