
using Gameplay.Enemies;
using Gameplay.Player;
using Timeline;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Food.Foodbeds
{
    public class Cacti : UniqueFoodbed, IDamageSource
    {
        [SerializeField] private CactiCollider cactiCollider;
        [SerializeField] private SpriteRenderer spikesRenderer;
        [SerializeField] private SpriteRenderer flowerRenderer;
        
        private Scriptable.Cacti Scriptable => (Scriptable.Cacti)scriptable;
        private int spikesLeft = 4;
        
        protected override void OnEatenByPlayer()
        {
            if (Amount == 4) flowerRenderer.enabled = false;
            
            UpdateSpikesSprite();
        }

        public void OnTouch(Collision2D col)
        {
            if (spikesLeft == 0) return;

            if (col.gameObject.TryGetComponent(out Enemy enemy))
                OnTouchEnemy(enemy);
            else if (col.gameObject.TryGetComponent(out PlayerManager player)) 
                OnTouchPlayer(player, col);
            
            spikesLeft--;
            UpdateSpikesSprite();

            if (spikesLeft == 0) cactiCollider.gameObject.SetActive(false);
        }

        private void OnTouchPlayer(IDamageable player, Collision2D col)
        {
            player.Damage(
                new DamageInstance(
                    new DamageSource(this, col.GetHashCode()),
                    Scriptable.ContactDamage, 
                    transform.position,
                    Scriptable.Knockback,
                    piercing: true
                ));
        }

        private void OnTouchEnemy(Enemy enemy)
        {
            enemy.Die();
        }

        private void UpdateSpikesSprite()
        {
            spikesRenderer.sprite = spikesLeft == 0 
                ? Scriptable.GetSpikeOnEatenSprite(Amount) 
                : Scriptable.GetSpikeOnTouchSprite(spikesLeft);
        }

        public override bool CanInteract() => spikesLeft == 0;

        public override bool CanSpawn(float random) => base.CanSpawn(random) && TimeManager.DayCounter > 1 && random < 1 / 3f;
        
        protected override bool CreateNotification => spikesLeft == 0;
    }
}