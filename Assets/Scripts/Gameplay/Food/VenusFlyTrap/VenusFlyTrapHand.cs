using Gameplay.Enemies;
using Player;
using UnityEngine;

namespace Gameplay.Food.VenusFlyTrap
{
    public class VenusFlyTrapHand : MonoBehaviour
    {
        [SerializeField] private VenusFlyTrap flyTrap;
        [SerializeField] private Sprite catchedSprite;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private new Collider2D collider;

        private void Awake() => collider = GetComponent<Collider2D>();

        private void OnTriggerEnter2D(Collider2D col)
        {
            bool catched = false;
            if (col.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.Die();
                catched = true;
            }
            else if (col.gameObject.TryGetComponent(out PlayerManager player))
            {
                player.Die(false);
                catched = true;
            }
            
            if(!catched) return;
            spriteRenderer.sprite = catchedSprite;
            collider.enabled = false;
            flyTrap.Catch();            
        }
    }
}