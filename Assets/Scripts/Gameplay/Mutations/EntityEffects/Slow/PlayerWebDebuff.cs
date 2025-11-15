using Gameplay.Mutations.Active;
using UnityEngine;

namespace Gameplay.Mutations.EntityEffects.Slow
{
    public class PlayerWebDebuff : SlowEntityEffect
    {
        private ParticleSystem particles;
        
        private void Awake()
        {
            particles = Instantiate(SpinneretGlands.DebuffParticles, transform);
        }

        protected override void OnApplied()
        {
            base.OnApplied();
            particles.Play();
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            particles.Stop();
        }
    }
}