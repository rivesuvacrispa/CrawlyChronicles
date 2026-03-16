using UnityEngine;

namespace Gameplay.Player
{
    public class DashPlayerAttack : BasePlayerAttack
    {
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private new ParticleSystem particleSystem;


        public void UpdateSize(float size)
        {
            transform.localScale = Vector3.one * size; 
            var main = particleSystem.main;
            main.startSize = size;
        }
        
        protected override void ApplyGradient(Gradient g)
        {
            Gradient newColor = new Gradient();
            newColor.SetKeys(g.colorKeys, defaultGradient.alphaKeys);
            var color = particles.colorOverLifetime;
            color.color = newColor;
        }

        public override void Enable()
        {
            base.Enable();
            particleSystem.Play();
        }

        public override void Disable()
        {
            base.Disable();
            particleSystem.Stop();
        }
    }
}