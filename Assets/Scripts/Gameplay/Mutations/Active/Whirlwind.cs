using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Player;
using UI.Menus;
using UnityEngine;

namespace Gameplay.Mutations.Active
{
    public class Whirlwind : ActiveAbility
    {
        [SerializeField, Range(1, 10)] private float durationLvl1; 
        [SerializeField, Range(1, 10)] private float durationLvl10;
        
        private CancellationTokenSource cancellationTokenSource;
        private float duration;

        
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            duration = LerpLevel(durationLvl1, durationLvl10, lvl);
        }


        protected override void Awake()
        {
            base.Awake();
            cancellationTokenSource = new CancellationTokenSource();
        }
        
        public override void Activate(bool auto = false)
        {
            ActivateTask(CreateCommonCancellationToken(cancellationTokenSource.Token))
                .Forget();
        }
        
        private async UniTask ActivateTask(CancellationToken cancellationToken)
        {
            await AttackController.Instance.ComboAttack(duration, cancellationToken)
                .SuppressCancellationThrow();
            SetOnCooldown();
        }

        protected override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            return null;
        }
    }
}