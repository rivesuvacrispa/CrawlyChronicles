using UnityEngine;

namespace Pooling
{
    public abstract class Poolable : MonoBehaviour
    {
        public IObjectPool ObjectPool { get; set; }


        
        public virtual void OnPool() => gameObject.SetActive(false);

        public virtual void OnFirstInstantiated()
        {
            
        }
        
        public virtual bool OnTakenFromPool(object data)
        {
            gameObject.SetActive(true);
            return true;
        }

        public void Pool()
        {
            ObjectPool?.Pool(this);
        }
    }
}