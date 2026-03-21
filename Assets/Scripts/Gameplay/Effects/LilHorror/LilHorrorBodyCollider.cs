using Gameplay.Enemies.Enemies;
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
            if (col.TryGetComponent(out DamageableEnemyHitbox hitbox) && hitbox.Enemy is not NeutralAnt)
                hitbox.Enemy.Damage(new DamageSource(this), LegTremor.ContactDamage, transform.position);
        }
    }
}