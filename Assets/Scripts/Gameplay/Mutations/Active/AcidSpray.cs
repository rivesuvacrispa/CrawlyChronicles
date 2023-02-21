using Gameplay.Enemies;
using UnityEngine;

namespace Gameplay.Abilities.Active
{
    public class AcidSpray : ActiveAbility
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [Header("Particles amount")] 
        [SerializeField] private int amountLvl1;
        [SerializeField] private int amountLvl10;
        [Header("Spray angle")] 
        [SerializeField] private int angleLvl1;
        [SerializeField] private int angleLvl10;
        [Header("Stun and knockback")] 
        [SerializeField, Range(0, 1)] private float stunDuration = 0.5f;
        [SerializeField, Range(0, 10)]  private float knockbackPower = 0.5f;
        

        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(particleSystem.isPlaying) particleSystem.Stop();
            var emission = particleSystem.emission;
            var shape = particleSystem.shape;
            shape.angle = LerpLevel(amountLvl1, amountLvl10, lvl);
            emission.SetBurst(0, new ParticleSystem.Burst(0, LerpLevel(angleLvl1, angleLvl10, lvl)));
        }

        public override void Activate() => particleSystem.Play();

        private void OnParticleCollision(GameObject other)
        {
            if (other.TryGetComponent(out Enemy enemy))
            {
                enemy.Damage(1, knockbackPower, stunDuration);
            }
        }
    }
}