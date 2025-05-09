using System;
using UnityEngine;

namespace Game
{
    [Flags]
    public enum RotationAxis
    {
        X = 1,
        Y = 2,
        Z = 4
    }
    public interface IRotatable
    {
        void Rotate(GameObject obj, Vector3 dir, RotationAxis rotationAxis);
    }
}