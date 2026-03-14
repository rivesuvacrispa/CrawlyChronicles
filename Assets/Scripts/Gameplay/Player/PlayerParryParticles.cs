using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerParryParticles : MonoBehaviour
    {
        private static PlayerParryParticles instance;
        
        [SerializeField] private ParticleSystem parryParticles;

        private PlayerParryParticles() => instance = this;
        
        public static void Play(Vector2 point)
        {
            instance.transform.position = point;
            instance.parryParticles.Play();
        }
    }
}