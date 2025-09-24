using System;
using Gameplay.Breeding;
using Gameplay.Enemies.Enemies;
using UnityEngine;

namespace Gameplay.Player
{
    public class BreedingAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem breedingParticles;
        
        private readonly int breedAnimHash = Animator.StringToHash("PlayerBodyBreeding");
        private readonly int idleAnimHash = Animator.StringToHash("PlayerBodyIdle");


        private void OnEnable()
        {
            BreedingManager.OnBecomePregnant += OnBecomePregnant;
            BreedingManager.OnEggsLaid += OnEggsLaid;
            BreedingManager.OnAbortion += OnAbortion;
            NeutralAnt.OnInteractionEnded += PlayIdleAnimation;
            NeutralAnt.OnInteractionStarted += PlayBreedingAnimation;
        }

        private void OnDisable()
        {
            BreedingManager.OnBecomePregnant -= OnBecomePregnant;
            BreedingManager.OnEggsLaid -= OnEggsLaid;
            BreedingManager.OnAbortion -= OnAbortion;
            NeutralAnt.OnInteractionEnded -= PlayIdleAnimation;
            NeutralAnt.OnInteractionStarted -= PlayBreedingAnimation;
        }

        private void OnAbortion() => StopParticles();

        private void OnEggsLaid(int _) => StopParticles();

        private void OnBecomePregnant() => PlayParticles();

        private void PlayParticles() => breedingParticles.Play();
        private void StopParticles() => breedingParticles.Stop();

        private void PlayBreedingAnimation()
        {
            animator.Play(breedAnimHash);
            PlayParticles();
        }

        private void PlayIdleAnimation()
        {
            animator.Play(idleAnimHash);
            StopParticles();
        }
    }
}