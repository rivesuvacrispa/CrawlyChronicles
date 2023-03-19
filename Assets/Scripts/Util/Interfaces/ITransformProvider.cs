using UnityEngine;

namespace Util.Interfaces
{
    public interface ITransformProvider : IDestructionEventProvider
    {
        public Transform Transform { get; }
    }
}