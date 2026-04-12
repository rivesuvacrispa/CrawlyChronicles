using System;
using Hitboxes;
using Pooling;
using UnityEngine;

namespace Util.Particles
{
    [RequireComponent(typeof(ParticleSystem)), RequireComponent(typeof(ParticleTriggerPool))]
    public class ParticleTriggerProvider : MonoBehaviour
    {
        [SerializeField] private float colliderSize;
        
        private ParticleTriggerPool pool;
        private ParticleSystem.Particle[] particles;
        private ParticleTrigger[] triggerPool;
        private new ParticleSystem particleSystem;
        private int maxParticles;

        public delegate void TriggerEvent(IDamageable col, int triggerId);
        public event TriggerEvent OnTrigger;
        
        
        
        private void Awake()
        {
            pool = GetComponent<ParticleTriggerPool>();
            particleSystem = GetComponent<ParticleSystem>();
            maxParticles = GetMaxParticles();
            particles = new ParticleSystem.Particle[maxParticles];
            triggerPool = new ParticleTrigger[maxParticles];
            
            for (int i = 0; i < maxParticles; i++)
            {
                ParticleTrigger p = (ParticleTrigger) pool.GetEffectObject(colliderSize);
                triggerPool[i] = p;
                p.SetActive(false);
                p.OnTrigger += OnParticleTriggered;
                p.OnTakenFromPool(colliderSize);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < maxParticles; i++) 
                triggerPool[i].OnTrigger -= OnParticleTriggered;
        }

        private void OnParticleTriggered(IDamageable col, int triggerId)
        {
            OnTrigger?.Invoke(col, triggerId);
        }

        private void FixedUpdate()
        {
            int numParticlesAlive = particleSystem.GetParticles(particles);
            for (int i = 0; i < maxParticles; i++)
            {
                if (i < numParticlesAlive)
                {
                    IPoolable p = triggerPool[i];
                    p.GameObject.transform.position = particles[i].position;
                    triggerPool[i].SetActive(true);
                }
                else
                {
                    triggerPool[i].SetActive(false);
                }
            }
        }

        private int GetMaxParticles()
        {
            int max = particleSystem.main.maxParticles;
            if (max == 1000)
            {
                Debug.LogWarning($"ParticleTriggerProvider: {gameObject.name}'s Particle System maxParticles is set to default value ({maxParticles}), it probably should be lower for ParticleTriggerProvider");
            }

            return max;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, colliderSize);
        }

        private void OnValidate()
        {
            particleSystem = GetComponent<ParticleSystem>();
            GetMaxParticles();
        }
    }
}