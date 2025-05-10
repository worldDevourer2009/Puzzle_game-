using System;
using UnityEngine;

namespace Game
{
    public interface IEntity
    {
        GameObject EntityGA { get; }
        
        Transform RightHandTransform { get; }
        Transform LeftHandTransform { get; }
        
        event Action<Vector3> OnMove;
        event Action OnJump;
        event Action OnUse;
        event Action<Vector3> OnRun;
        event Action OnIdle;
    }
}