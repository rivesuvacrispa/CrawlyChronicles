using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Player;
using Pooling;
using UnityEngine;
using Util;

namespace Gameplay.Effects.PhantomPlayerAttack
{
    public class PhantomPlayerAttack : BasePlayerAttack, IPoolable
    {
        // TODO: pool on main menu reset
        public IObjectPool ObjectPool { get; set; }
        public GameObject GameObject => gameObject;
        
        public void OnPool()
        {
            Disable();
        }

        public void OnFirstInstantiated()
        {
            
        }

        public bool OnTakenFromPool(object data)
        {
            if (data is not PhantomPlayerAttackArguments args) return false;
            
            if (args.bonusDamage > 0)
                AddBonusDamage(args.bonusDamage);
            
            transform.position = args.spawnPos;
            Vector3 finalPos = 2 * args.targetPos - args.spawnPos;
            Vector3 desiredUp = (args.targetPos - args.spawnPos).normalized;
            transform.rotation = Quaternion.LookRotation(transform.forward, desiredUp);
            
            trailRenderer.Clear();

            transform.DOMove(finalPos, args.lifetime * PlayerSizeManager.CurrentSize)
                .OnComplete(() => ((IPoolable)this).Pool())
                .AsyncWaitForCompletion()
                .AsUniTask()
                .AttachExternalCancellation(gameObject.CreateCommonCancellationToken());
            
            Enable();
            return true;
        }
    }
}