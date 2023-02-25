using UnityEngine;

namespace Util
{
    public interface ITransformProvider : IDestructionEventProvider
    {
        public Transform Transform { get; }
    }
}