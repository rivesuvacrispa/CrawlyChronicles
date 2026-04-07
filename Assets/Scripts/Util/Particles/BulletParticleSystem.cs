using Gameplay.Player;
using UnityEngine;

namespace Util.Particles
{
    [RequireComponent(typeof(ParticleSystem))]
    public class BulletParticleSystem : MonoBehaviour
    {
        [SerializeField] private bool autoSize = true;
        [SerializeField] private bool autoAmount = true;

        private float baseAmount = 0;
        private ParticleSystem.MinMaxCurve baseSize = 0;
        private bool hasBurst;
        private bool hasRateOverTime;
        private bool hasRateOverDistance;
        private ParticleSystem.Burst initialBurst;

        private ParticleSystem ps;

        public ParticleSystem Particles
        {
            get
            {
#if UNITY_EDITOR
                return ps is null ? GetComponent<ParticleSystem>() : ps;
#endif
                return ps;
            }
            private set { ps = value; }
        }

        private ParticleSystem.EmissionModule emission;
        private ParticleSystem.MainModule main;
        private ParticleSystem.ShapeModule shape;


        private void Awake()
        {
            Particles = GetComponent<ParticleSystem>();
            emission = Particles.emission;
            main = Particles.main;
            shape = Particles.shape;

            hasBurst = emission.burstCount != 0;
            hasRateOverTime = emission.rateOverTime.Evaluate(0) > 0f;
            hasRateOverDistance = emission.rateOverDistance.Evaluate(0) > 0f;

            if (hasBurst)
                initialBurst = emission.GetBurst(0);

            if (baseAmount == 0)
                baseAmount = hasBurst
                    ? emission.GetBurst(0).count.Evaluate(0f)
                    : hasRateOverTime
                        ? emission.rateOverTime.Evaluate(0f)
                        : emission.rateOverDistance.Evaluate(0f);

            if (baseSize.constant == 0)
            {
                baseSize = main.startSize;
            }
        }

        public void SetBaseAmount(float amount)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying )
            {
                Awake();
            }
#endif
            
            baseAmount = amount;
            if (hasBurst)
            {
                var i = initialBurst.count;
                float ratio = i.constantMin / (i.constantMax == 0 ? i.constantMin : i.constantMax);
                initialBurst.count = new ParticleSystem.MinMaxCurve(amount * ratio, amount);
                print($"Burst bounds set to {amount * ratio}, {amount}");
            }
            
            RecalculateAmount();
        }

        public void SetBaseSize(ParticleSystem.MinMaxCurve size)
        {
            baseSize = size;
            RecalculateSize();
        }

        private void OnEnable()
        {
            PlayerManager.OnStatsChanged += OnPlayerStatsChanged;
            RecalculateAmount();
            RecalculateSize();
        }

        private void OnDisable()
        {
            PlayerManager.OnStatsChanged -= OnPlayerStatsChanged;
        }

        private void OnPlayerStatsChanged(PlayerStats changes)
        {
            if (changes.ProjectileSize != 0)
                RecalculateSize();

            if (changes.ProjectileAmount != 0)
                RecalculateAmount();
        }

        private void RecalculateSize()
        {
            if (!autoSize) return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Awake();
            }
#endif

            float mult = 1 + PlayerManager.PlayerStats.ProjectileSize;
            main.startSize = new ParticleSystem.MinMaxCurve(
                baseSize.constantMin * mult,
                baseSize.constantMax * mult
            );
        }

        private void RecalculateAmount()
        {
            if (!autoAmount) return;
            
            PlayerStats current = PlayerManager.PlayerStats;
            float mult = 1 + current.ProjectileAmount;
            float amount = baseAmount * mult;

            if (hasRateOverTime)
                emission.rateOverTime = amount;

            if (hasRateOverDistance)
                emission.rateOverDistance = amount;

            if (hasBurst)
            {
                var initCurve = initialBurst.count;
                var newCurve = new ParticleSystem.MinMaxCurve(
                    initCurve.constantMin * mult,
                    initCurve.constantMax * mult
                );

                ParticleSystem.Burst b = new ParticleSystem.Burst(
                    initialBurst.time,
                    newCurve,
                    initialBurst.cycleCount,
                    initialBurst.repeatInterval
                );
                emission.SetBurst(0, b);
            }
        }

        public void SetShapeAngle(float angle)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                shape = Particles.shape;
            }
#endif
            
            shape.angle = angle;
        }

        public void SetDuration(float duration)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                main = Particles.main;
            }
#endif
            
            main.duration = duration;
        }
    }
}