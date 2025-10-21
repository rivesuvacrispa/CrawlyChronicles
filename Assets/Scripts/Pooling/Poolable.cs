using UnityEngine;

namespace Pooling
{
    public abstract class Poolable : MonoBehaviour
    {
        public IObjectPool ObjectPool { get; set; }


        
        public virtual void OnPool()
        {
#if UNITY_EDITOR
            // Suppress annoying errors on editor game window playmode end
            try
            {
                gameObject.SetActive(false);
            } catch {}
#else
            gameObject.SetActive(false);
#endif
        }

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