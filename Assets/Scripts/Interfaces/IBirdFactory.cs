using UnityEngine;

namespace Interfaces
{
    public interface IBirdFactory
    {
        GameObject CreateRandomBird(float minY, float maxY);
        bool LastDirection { get; }
    }
}