using UnityEngine;

namespace Gameplay.Abilities
{
    public class AcidSpray : Ability
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [Header("Particles amount")] 
        [SerializeField] private int amountLvl1;
        [SerializeField] private int amountLvl10;
        [Header("Spray angle")] 
        [SerializeField] private int angleLvl1;
        [SerializeField] private int angleLvl10;
        

        
        public override void OnLevelChanged(int lvl)
        {
            var emission = particleSystem.emission;
            var shape = particleSystem.shape;
            shape.angle = LerpLevel(amountLvl1, amountLvl10, lvl);
            emission.SetBurst(0, new ParticleSystem.Burst(0, LerpLevel(angleLvl1, angleLvl10, lvl)));
        }

        public override void Activate() => particleSystem.Play();

        /*private void OnParticleCollision(GameObject other)
        {
            Debug.Log($"Posion! on {other.name}");
        }*/
    }
}