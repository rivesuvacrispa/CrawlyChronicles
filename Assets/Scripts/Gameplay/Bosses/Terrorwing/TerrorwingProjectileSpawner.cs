using UnityEngine;

namespace Scripts.Gameplay.Bosses.Terrorwing
{
    public class TerrorwingProjectileSpawner : MonoBehaviour
    {
        public TerrorwingProjectile Spawn(TerrorwingProjectile projectile)
        {
            var p = Instantiate(projectile);
            p.transform.position = transform.position;
            return p;
        }
    }
}