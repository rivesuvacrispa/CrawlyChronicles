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
            var pools = GetComponentsInChildren<IObjectPool>();
            foreach (IObjectPool pool in pools) 
                PoolsDict.Add(pool.GetPoolableType(), pool);
        }

        public static T GetEffect<T>(
            object data = null,
            Vector3 position = default,
            Quaternion rotation = default) where T : Poolable
        {
            return (T)PoolsDict[typeof(T)].GetEffectObject(data, position, rotation);
        }

        public static Poolable GetEffect(
            Type t, 
            object data = null,
            Vector3 position = default,
            Quaternion rotation = default)
        {
            return PoolsDict[t].GetEffectObject(data, position, rotation);
        }
    }
}