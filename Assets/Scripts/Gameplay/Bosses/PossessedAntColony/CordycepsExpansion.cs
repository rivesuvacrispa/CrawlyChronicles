using System;
using System.Collections;
using Definitions;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Util;

namespace Scripts.Gameplay.Bosses.PossessedAntColony
{
    public class CordycepsExpansion : MonoBehaviour
    {
        public static CordycepsExpansion Instance { get; private set; }

        [SerializeField] private Gradient globalColorGradient;
        [SerializeField] private ParticleSystem fogParticles;
        [SerializeField] private int maxExpansionLevel;
        [SerializeField] private Sprite[] cordycepsSprites = new Sprite[4];
        [SerializeField] private float cordycepsGrowthTime;
        [SerializeField, ShowOnly] private int currentExpantionLevel;
        
        private float previousExpantionLevel;
        private ParticleSystem.EmissionModule fogEmission;
        private ColorAdjustments environmentColor;
        private float environmentUpdateThreshhold;
        
        public Sprite GetCordycepsSprite(int growthStage) =>
            cordycepsSprites[Math.Clamp(growthStage, 0, 4)];

        public float CordycepsGrowthTime => cordycepsGrowthTime;
        
        
        
        private CordycepsExpansion() => Instance = this;
        
        private void OnEnable()
        {
            fogEmission = fogParticles.emission;
            environmentUpdateThreshhold = maxExpansionLevel / 20f;
            GlobalDefinitions.GlobalVolumeProfile.TryGet(out environmentColor);
            StartCoroutine(UpdateEnvironmentRoutine());
        }

        private void OnDisable()
        {
            currentExpantionLevel = 0;
            UpdateEnvironment();
        }

        private IEnumerator UpdateEnvironmentRoutine()
        {
            while (enabled)
            {
                if(Mathf.Abs(previousExpantionLevel - currentExpantionLevel) >= environmentUpdateThreshhold)
                    UpdateEnvironment();
                yield return new WaitForSeconds(3f);
            }
        }
        
        public void AddExpansion(int amount)
        {
            if(!enabled) return;
            if (currentExpantionLevel == 0) StartCoroutine(UpdateEnvironmentRoutine());
            currentExpantionLevel += amount;
            if (currentExpantionLevel > maxExpansionLevel) currentExpantionLevel = maxExpansionLevel;
        }

        private void UpdateEnvironment()
        {
            if (currentExpantionLevel == 0)
            {
                environmentColor.colorFilter.value = globalColorGradient.Evaluate(0);
                fogParticles.Stop();
                return;
            }

            if(!fogParticles.isPlaying) fogParticles.Play();

            float normalized = Mathf.Clamp01((float) currentExpantionLevel / maxExpansionLevel);
            environmentColor.colorFilter.value = globalColorGradient.Evaluate(normalized);
            fogEmission.rateOverTime = normalized * 8;
        }
    }
}