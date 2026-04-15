using System.Threading;
using Camera;
using Cysharp.Threading.Tasks;
using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util;
using Util.Abilities;
using Util.Attributes;

namespace Gameplay.Mutations.Active
{
    public class DashAttack : ActiveAbility
    {
        [Header("References")] 

        [SerializeField] private DashPlayerAttack attack;
        [SerializeField, MinMaxRange(0, 10f)] private LevelFloat force = new LevelFloat(new Vector2(3, 10));
        [SerializeField, MinMaxRange(0.1f, 10f)] private LevelFloat size = new LevelFloat(new Vector2(0.8f, 3f));

        
        private float currentForce;
        private float currentSize;
        private int VoteSource => GetHashCode();

        

        // TODO: WALLS COLLIDER
        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerSizeManager.OnSizeChanged += OnPlayerSizeChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerSizeManager.OnSizeChanged -= OnPlayerSizeChanged;
        }

        private void OnPlayerSizeChanged(float f)
        {
            currentSize = size.AtLvl(level) * PlayerSizeManager.CurrentSize;
            attack.UpdateSize(currentSize);
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentForce = force.AtLvl(lvl);
            currentSize = size.AtLvl(lvl) * PlayerSizeManager.CurrentSize;
            attack.UpdateSize(currentSize);
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            
            Vector2 mousePos = MainCamera.WorldMousePos;
            PlayerPhysicsBody.Rigidbody.RotateTowardsPosition(mousePos, 360);
            
            ActivateTask(CreateCommonCancellationToken()).Forget();
        }

        private async UniTask ActivateTask(CancellationToken cancellationToken)
        {
            await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
            
            AttackController.Instance.CancelAttack();
            PlayerMovement.CancelKnockback();
            
            attack.Enable();
            PlayerHitbox.Immune.Vote(VoteSource);
            PlayerMovement.CanMove = false;
            PlayerAnimator.LockState(PlayerAnimatorState.Idle);
            PlayerPhysicsBody.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            PlayerPhysicsBody.PhysicsCollider.enabled = false;
            
            await PlayerMovement.Dash(0.4f, currentForce, cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            
            PlayerHitbox.Immune.Unvote(VoteSource);
            PlayerMovement.CanMove = true;
            PlayerAnimator.UnlockState();
            PlayerPhysicsBody.ResetConstraints();
            PlayerPhysicsBody.PhysicsCollider.enabled = true;
            attack.Disable();
        }

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                Scriptable.Cooldown,
                force.UseKey(LevelFieldKeys.ATTACK_FORCE),
                size.UseKey(LevelFieldKeys.ATTACK_SIZE)
            };
        }
    }
}