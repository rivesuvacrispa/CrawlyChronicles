using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public abstract class ObjectPool<T>  : MonoBehaviour, IObjectPool where T : IPoolable
    {
        [SerializeField] private T prefab;

        private readonly Stack<T> objectStack = new();
        private int instanceCounter;

        
        
        public Type GetPoolableType() => typeof(T);

        public IPoolable GetEffectObject(object data, Vector3? position = null, Quaternion? rotation = null)
        {
            bool popped = objectStack.TryPop(out T pop);
            T obj = popped ? pop : Instantiate(prefab.GameObject).GetComponent<T>();
            
            if (!popped)
            {
                instanceCounter++;
                obj.OnFirstInstantiated();
                obj.GameObject.name = $"{prefab.GameObject.name}_{instanceCounter}";
            }

            if (position is not null)
                obj.GameObject.transform.position = position.Value;
            
            if (rotation is not null)
                obj.GameObject.transform.rotation = rotation.Value;
            
            if (!obj.OnTakenFromPool(data))
            {
                Pool(obj);
                return null;
            };
            
            if (popped) return obj;
            
            obj.ObjectPool = this;
            return obj; 
        }

        public void Pool(IPoolable effectObject)
        {
            if(effectObject is not T obj) return;
            objectStack.Push(obj);
            obj.OnPool();
        }
    }
}