using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public abstract class ObjectPool<T>  : MonoBehaviour, IObjectPool where T : Poolable
    {
        [SerializeField] private T prefab;

        private readonly Stack<T> objectStack = new();

        
        
        public Type GetPoolableType() => typeof(T);

        public Poolable GetEffectObject(object data, Vector3 position, Quaternion rotation)
        {
            bool popped = objectStack.TryPop(out T pop);
            T obj = popped ? pop : Instantiate(prefab);
            obj.OnFirstInstantiated();
            if (!obj.OnTakenFromPool(data))
            {
                Pool(obj);
                return null;
            };
            
            if (!position.Equals(default))
                obj.transform.position = position;
            if (!rotation.Equals(default))
                obj.transform.rotation = rotation;
            if (popped) return obj;
            
            obj.ObjectPool = this;
            OnInstantiated(obj);
            return obj; 
        }

        protected virtual void OnInstantiated(T obj)
        {
        }

        public void Pool(Poolable effectObject)
        {
            if(effectObject is not T obj) return;
            objectStack.Push(obj);
            obj.OnPool();
        }
    }
}