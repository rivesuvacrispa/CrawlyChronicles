using Gameplay.Breeding;
using Gameplay.Food;
using UnityEngine;

namespace Gameplay.Enemies.Enemies
{
    public abstract class SidefacedEnemy : Enemy
    {
        [SerializeField] private Transform bodySpriteTransform;
        
        private float prevX;

        public abstract override void OnMapEntered();

        public abstract override void OnPlayerLocated();

        public abstract override void OnEggsLocated(EggBed eggBed);

        public abstract override void OnFoodLocated(Foodbed foodBed);
        protected abstract override void DamageTaken();
        
        private void LateUpdate()
        {
            float currentX = rb.position.x;
            float diff = currentX - prevX;
            
            Vector3 scale = Vector3.one;
            if (diff < -0.01)
            {
                scale.x = -1;
                bodySpriteTransform.localScale = scale;
            }
            else if (diff > 0.01)
            {
                scale.x = 1;
                bodySpriteTransform.localScale = scale;
            }

            prevX = currentX;
        }
    }
}