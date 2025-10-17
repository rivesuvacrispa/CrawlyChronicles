using System;
using UnityEngine;

namespace Pooling
{
    public interface IObjectPool
    {
        public Type GetPoolableType();
        public Poolable GetEffectObject(object data, Vector3 position, Quaternion rotation);
        public void Pool(Poolable effectObject);
    }
}