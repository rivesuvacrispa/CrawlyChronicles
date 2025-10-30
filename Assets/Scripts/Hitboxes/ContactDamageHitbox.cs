using Gameplay.Enemies;
using Gameplay.Player;
using UnityEngine;
using Util.Interfaces;

namespace Hitboxes
{
    public class ContactDamageHitbox : MonoBehaviour
    {
        [SerializeField] private Component contactDamageProvider;

        private IContactDamageProvider provider;
        
        private void Awake()
        {
            if (contactDamageProvider is IContactDamageProvider e)
                provider = e;
            else
            {
                Debug.LogError($"Component {contactDamageProvider.name} is not IContactDamageProvider");
                gameObject.SetActive(false);
            }
        }
        
        /**
         * PlayerHitbox x EnemyAttack
         * Trigger contact damage
         */
        private void OnTriggerEnter2D(Collider2D col)
        {
            TryDealContactDamage(col);
        }

        private void TryDealContactDamage(Collider2D col)
        {
            if (!col.gameObject.TryGetComponent(out PlayerHitbox _)) return;
            
            ((IDamageable)PlayerManager.Instance).Damage(
                new DamageInstance(
                    new DamageSource(provider),
                    provider.ContactDamage,
                    provider.ContactDamagePosition,
                    provider.ContactDamageKnockback, 
                    provider.ContactDamageStunDuration, 
                    provider.ContactDamageColor, 
                    provider.ContactDamagePiercing));
        }

    }
}