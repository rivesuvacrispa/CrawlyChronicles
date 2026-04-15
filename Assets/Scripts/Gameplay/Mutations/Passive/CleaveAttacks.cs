using System.Collections.Generic;
using Definitions;
using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;
using Util.Interfaces;

namespace Gameplay.Mutations.Passive
{
    public class CleaveAttacks : BasicAbility, IDamageSource
    {
        [Header("References")]
        [SerializeField] private ParticleSystem particles;
        [SerializeField, MinMaxRange(0, 3f)] private LevelFloat radius = new LevelFloat(0.5f, 1.25f);
        [SerializeField, MinMaxRange(0, 3f)] private LevelFloat damage = new LevelFloat(0.5f, 2f);
        
        private static readonly List<Collider2D> OverlapResults = new(32);
        private float currentRadius;
        private float currentDamagePercent;

        

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentRadius = radius.AtLvl(lvl);
            currentDamagePercent = damage.AtLvl(lvl);
            
            // TODO: benify from player size
            ParticleSystem.MainModule main = particles.main;
            main.startSize = LerpLevel(2f, 4f, lvl);
            main.startSpeed = LerpLevel(3f, 5f, lvl);
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            DamageableHitbox.OnCollideWithPlayerAttack += OnCollideWithPlayerAttack;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DamageableHitbox.OnCollideWithPlayerAttack -= OnCollideWithPlayerAttack;
        }

        private void OnCollideWithPlayerAttack(IDamageable enemy, BasePlayerAttack attack, float damageDealt)
        {
            if (damageDealt == 0) return;

            Transform attackTransform = attack.transform;
            Vector3 attackPos = attackTransform.position;
            Vector3 attackFwd = attackTransform.up;

            var pTransform = particles.transform;
            pTransform.position = attackPos;
            pTransform.forward = attackFwd;            
            particles.Play();


            Vector3 cleaveCenter = attackPos + attackFwd * currentRadius;
            int contacts = Physics2D.OverlapCircle(
                cleaveCenter,
                currentRadius,
                GlobalDefinitions.EnemyPhysicsContactFilter, OverlapResults);

            for (var i = 0; i < Mathf.Min(contacts, 32); i++)
            {
                var c = OverlapResults[i];

                if (c.TryGetComponent(out IDamageableEnemy e))
                {
                    if (e.Equals(enemy)) continue;
                    
                    e.Damage(new DamageInstance(new DamageSource(this, Time.frameCount),
                        damageDealt * currentDamagePercent, attackPos, piercing: true));
                }
            }
        }
        
        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                radius.UseKey(LevelFieldKeys.EFFECT_RANGE),
                damage.UseKey(LevelFieldKeys.DAMAGE).UseFormatter(StatFormatter.PERCENT)
            };
        }


        private void OnDrawGizmos()
        {
            var tr = transform;
            Vector3 playerPos = tr.position;
            Vector3 cleavePos = playerPos + tr.forward.normalized * currentRadius;
            Gizmos.DrawWireSphere(cleavePos, currentRadius);
        }
    }
}