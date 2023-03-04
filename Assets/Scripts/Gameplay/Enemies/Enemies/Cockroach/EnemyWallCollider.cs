using Definitions;
using UnityEngine;

namespace Gameplay.Enemies
{
    public class EnemyWallCollider : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;

        private void Start()
        {
            enemy.WallCollider = this;
            enabled = false;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if(col.collider.gameObject.layer.Equals(GlobalDefinitions.DefaultLayerMask))
                enemy.OnWallCollision();
        }
    }
}