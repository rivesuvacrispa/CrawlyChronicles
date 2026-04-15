using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Player;
using UI.Menus;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;

namespace Gameplay.Mutations.Active
{
    public class Whirlwind : ActiveAbility
    {
        [SerializeField, MinMaxRange(1f, 10f)] private LevelFloat duration = new LevelFloat(1.5f, 3.6f);
        
        private CancellationTokenSource cancellationTokenSource;
        private float currentDuration;

        
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentDuration = duration.AtLvl(lvl);
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
            AttackController.Instance.CancelAttack();
            await AttackController.Instance.WhirlwindAttack(currentDuration, cancellationToken)
                .SuppressCancellationThrow();
            SetOnCooldown();
        }

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                Scriptable.Cooldown,
                duration.UseKey(LevelFieldKeys.DURATION).UseFormatter(StatFormatter.SECONDS)
            };
        }
    }
}