using System;
using UnityEngine;

namespace Pooling
{
    public interface IPoolable
    {
        public IObjectPool ObjectPool { get; set; }

        public GameObject GameObject { get; }
        public void OnPool();
        public void OnFirstInstantiated();
        public bool OnTakenFromPool(object data);
        public void Pool()
        {
            ObjectPool?.Pool(this);
        }
    }
}