using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;

namespace Gameplay.Mutations.Active
{
    public class Dig : ActiveAbility
    {
        [SerializeField] private Transform spriteTransform;
        [SerializeField] private LevelConst immunityDuration = new LevelConst(0.5f);
        [SerializeField, MinMaxRange(1f, 10f)] private LevelFloat duration = new LevelFloat(new Vector2(3f, 10f));

        private float currentDuration;
        private int VoteSource => GetHashCode();
        private bool active;
        private CancellationTokenSource cancellationTokenSource;


        

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

        public override bool CanActivate() => active || CurrentCooldown <= 0;

        public override void Activate(bool auto = false)
        {
            if (active)
            {
                // Suppress autocast so it doesn't automatically dig you out
                if (auto) return;
                
                cancellationTokenSource?.Cancel();
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
                return;
            }
            
            ActivateTask(CreateCommonCancellationToken(cancellationTokenSource.Token)).Forget();
        }

        private async UniTask ActivateTask(CancellationToken cancellationToken)
        {
            active = true;
            PlayerMovement.CancelKnockback();
            AttackController.Instance.CancelAttack();
            PlayerHitbox.Immune.Vote(VoteSource);
            spriteTransform.gameObject.SetActive(true);
            PlayerMovement.CanMove = false;
            PlayerAnimator.LockState(PlayerAnimatorState.None);
            PlayerPhysicsBody.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            PlayerPhysicsBody.PhysicsCollider.enabled = false;
            PlayerLocatorBody.Enabled = false;

            await UniTask.Delay(TimeSpan.FromSeconds(immunityDuration.Value), cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            PlayerHitbox.Immune.Unvote(VoteSource);

            await UniTask.Delay(TimeSpan.FromSeconds(currentDuration - immunityDuration.Value), cancellationToken: cancellationToken)
                .SuppressCancellationThrow();

            active = false;
            spriteTransform.gameObject.SetActive(false);
            PlayerMovement.CanMove = true;
            PlayerAnimator.UnlockState();
            PlayerPhysicsBody.ResetConstraints();
            PlayerPhysicsBody.PhysicsCollider.enabled = true;
            PlayerLocatorBody.Enabled = true;
            SetOnCooldown();
        }

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                Scriptable.Cooldown,
                immunityDuration.UseKey(LevelFieldKeys.IMMUNITY_DURATION).UseFormatter(StatFormatter.SECONDS),
                duration.UseKey(LevelFieldKeys.EFFECT_DURATION).UseFormatter(StatFormatter.SECONDS)
            };
        }
    }
}