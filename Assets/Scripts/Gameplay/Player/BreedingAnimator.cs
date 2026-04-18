using Gameplay.Breeding;
using Gameplay.Enemies.Enemies;
using UnityEngine;

namespace Gameplay.Player
{
    public class BreedingAnimator : MonoBehaviour
    {
        [SerializeField] private ParticleSystem breedingParticles;
        


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
            PlayerBodyAnimator.PlayBreed();
            PlayParticles();
        }

        private void PlayIdleAnimation()
        {
            PlayerBodyAnimator.PlayIdle();
            StopParticles();
        }
    }
}