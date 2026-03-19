using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pooling
{
    public abstract class Poolable : MonoBehaviour, IPoolable
    {
        public IObjectPool ObjectPool { get; set; }


        public GameObject GameObject => gameObject;

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

        protected async UniTask PoolTask(float duration, CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken);
            ((IPoolable)this).Pool();
        }
    }
}