using System.Collections.Generic;
using Definitions;
using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.Passive
{
    public class CleaveAttacks : BasicAbility, IDamageSource
    {
        [Header("References")]
        [SerializeField] private ParticleSystem particles;
        [Header("Cleave Radius")] 
        [SerializeField, Range(0, 3f)] private float radiusLvl1;
        [SerializeField, Range(0, 3f)] private float radiusLvl10;
        [Header("Damage")] 
        [SerializeField, Range(0, 3f)] private float damageLvl1;
        [SerializeField, Range(0, 3f)] private float damageLvl10;
        
        private static readonly List<Collider2D> OverlapResults = new(32);
        private float radius;
        private float damagePortion;

        
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            radius = LerpLevel(radiusLvl1, radiusLvl10, lvl);
            damagePortion = LerpLevel(damageLvl1, damageLvl10, lvl);
            
            ParticleSystem.MainModule main = particles.main;
            main.startSize = LerpLevel(2f, 4f, lvl);
            main.startSpeed = LerpLevel(3f, 5f, lvl);
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            DamageableEnemyHitbox.OnCollideWithPlayerAttack += OnCollideWithPlayerAttack;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DamageableEnemyHitbox.OnCollideWithPlayerAttack -= OnCollideWithPlayerAttack;
        }

        private void OnCollideWithPlayerAttack(IDamageable enemy, BasePlayerAttack attack, float damageDealt)
        {
            if (damageDealt == 0) return;

            Transform attackTransform = attack.transform;
            Vector3 attackPos = attackTransform.position;
            Vector3 attackFwd = attackTransform.up;
            
            particles.transform.position = attackPos;
            particles.transform.forward = attackFwd;            
            particles.Play();


            Vector3 cleaveCenter = attackPos + attackFwd * radius;
            int contacts = Physics2D.OverlapCircle(
                cleaveCenter,
                radius,
                GlobalDefinitions.EnemyPhysicsContactFilter, OverlapResults);

            for (var i = 0; i < Mathf.Min(contacts, 32); i++)
            {
                var c = OverlapResults[i];

                if (c.TryGetComponent(out IDamageableEnemy e))
                {
                    if (e.Equals(enemy)) continue;
                    
                    e.Damage(new DamageInstance(new DamageSource(this, Time.frameCount),
                        damageDealt * damagePortion, attackPos, piercing: true));
                }
            }
        }


        private void OnDrawGizmos()
        {
            Vector3 playerPos = transform.position;
            Vector3 cleavePos = playerPos + transform.forward.normalized * radius;
            Gizmos.DrawWireSphere(cleavePos, radius);
        }
    }
}