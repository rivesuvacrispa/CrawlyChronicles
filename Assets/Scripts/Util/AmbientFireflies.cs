using System.Collections.Generic;
using Timeline;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Util
{
    [RequireComponent(typeof(ParticleSystem))]
    public class AmbientFireflies : MonoBehaviour
    {
        [SerializeField] private GameObject lightPrefab;

        private new ParticleSystem particleSystem;
        private readonly List<Light2D> instances = new();
        private ParticleSystem.Particle[] particles;
        private const float INTENSITY = 1 / 256f;

        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
            particleSystem.Stop();
        }

        private void OnEnable()
        {
            if(TimeManager.IsDay) particleSystem.Stop(); 
            else particleSystem.Play();
        }

        private void LateUpdate()
        {
            int count = particleSystem.GetParticles(particles);

            while (instances.Count < count)
                instances.Add(Instantiate(lightPrefab, particleSystem.transform).GetComponent<Light2D>());

            bool worldSpace = (particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World);
            for (int i = 0; i < instances.Count; i++)
            {
                var inst = instances[i];
                var particle = particles[i];
                if (i < count)
                {
                    if (worldSpace) inst.transform.position = particles[i].position;
                    else inst.transform.localPosition = particles[i].position;
                    inst.intensity = particle.GetCurrentColor(particleSystem).a * INTENSITY;
                    inst.gameObject.SetActive(true);
                }
                else inst.gameObject.SetActive(false);
            }
        }
    }
}
