using UnityEngine;

namespace Gameplay.Food.Cacti
{
    public class CactiCollider : MonoBehaviour
    {
        [SerializeField] private Cacti cacti;
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            cacti.OnTouch(col);
        }
    }
}