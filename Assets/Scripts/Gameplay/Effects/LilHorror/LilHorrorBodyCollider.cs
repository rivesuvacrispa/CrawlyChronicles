using Gameplay.Enemies.Enemies;
using Gameplay.Mutations;
using Gameplay.Mutations.Passive;
using Hitboxes;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Effects.LilHorror
{
    public class LilHorrorBodyCollider : MonoBehaviour, IDamageSource, IFriendlyUnit
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent(out DamageableEnemyHitbox hitbox) && hitbox.Damageable is not NeutralAnt)
                hitbox.Damageable.Damage(
                    new DamageSource(this),
                    BasicAbility.CalculateSummonDamage(LegTremor.ContactDamage),
                    transform.position
                );
        }
    }
}