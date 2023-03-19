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
                enemy.Damage(999, transform.position, 0, 0, Color.white, true);
                catched = true;
            }
            else if (col.gameObject.TryGetComponent(out PlayerManager player))
            {
                player.Damage(999, transform.position, 0, ignoreArmor: true);
                catched = true;
            }
            
            if(!catched) return;
            spriteRenderer.sprite = catchedSprite;
            collider.enabled = false;
            flyTrap.Catch();            
        }
    }
}