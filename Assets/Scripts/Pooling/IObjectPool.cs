using System;
using UnityEngine;

namespace Pooling
{
    public interface IObjectPool
    {
        public Type GetPoolableType();
        public IPoolable GetEffectObject(object data, Vector3? position = null, Quaternion? rotation = null);
        public void Pool(IPoolable effectObject);
    }
}