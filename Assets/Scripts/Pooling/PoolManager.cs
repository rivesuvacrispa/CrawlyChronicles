using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public class PoolManager : MonoBehaviour
    {
        private static readonly Dictionary<Type, IObjectPool> PoolsDict = new();

        

        private void Awake()
        {
            Debug.Log("PoolManager Awake");
            var pools = GetComponentsInChildren<IObjectPool>();
            foreach (IObjectPool pool in pools) 
                PoolsDict.Add(pool.GetPoolableType(), pool);
        }

        public static T GetEffect<T>(
            object data = null,
            Vector3 position = default,
            Quaternion rotation = default) where T : IPoolable
        {
            return (T)PoolsDict[typeof(T)].GetEffectObject(data, position, rotation);
        }
    }
}