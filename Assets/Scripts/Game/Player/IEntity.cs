using System;
using Core;
using UnityEngine;

namespace Game
{
    public interface IEntity
    {
        GameObject EntityGA { get; }
        
        Transform RightHandTransform { get; }
        Transform LeftHandTransform { get; }
        Transform CenterBottomTransform { get; }
        
        event Action<Vector3, bool> OnMove;
        event Action OnJump;
        event Action OnUse;
        event Action OnIdle;
    }

    public interface IPlayerFacade : IEntity
    {
        Transform EyesTransform { get; }
        Transform BottomFoot { get; }
        Transform TopFoot { get; }
        void Initialize(PlayerStats stats);
        void Move(Vector3 direction, bool run = false);
        void Jump();
        void Use();
        void Idle();
        void Look(Vector3 lookDirection);
    }
}