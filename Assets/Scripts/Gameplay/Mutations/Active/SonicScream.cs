using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.Active
{
    public class SonicScream : ActiveAbility, IDamageSource
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [Header("Waves amount")] 
        [SerializeField] private int wavesLvl1;
        [SerializeField] private int wavesLvl10;
        [Header("Waves lifetime")] 
        [SerializeField] private float lifetimeLvl1;
        [SerializeField] private float lifetimeLvl10;
        [Header("Stun duration")] 
        [SerializeField] private float stunLvl1;
        [SerializeField] private float stunLvl10;
        [Header("Knockback")] 
        [SerializeField] private float knockbackLvl1;
        [SerializeField] private float knockbackLvl10;

        private float stun;
        private float knockback;
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(particleSystem.isPlaying) particleSystem.Stop();
            stun = LerpLevel(stunLvl1, stunLvl10, lvl);
            knockback = LerpLevel(knockbackLvl1, knockbackLvl10, lvl);
            var emission = particleSystem.emission;
            var main = particleSystem.main;
            int waves = Mathf.FloorToInt(LerpLevel(wavesLvl1, wavesLvl10, lvl));
            emission.SetBurst(0, new ParticleSystem.Burst(0, 15, 15, waves, 0.2f));
            float lifetime = LerpLevel(lifetimeLvl1, lifetimeLvl10, lvl);
            main.startLifetime = new ParticleSystem.MinMaxCurve(lifetime, lifetime + 0.25f);
        }

        protected override void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            damageable.Damage(new DamageInstance(new DamageSource(this), 
                0,
                PlayerPhysicsBody.Position,
                knockback,
                stun,
                Color.pink));
        }
        
        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            particleSystem.Play();
        }

        protected override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            float cd = Scriptable.GetCooldown(lvl);
            float prevCd = cd;
            float waves = Mathf.FloorToInt(LerpLevel(wavesLvl1, wavesLvl10, lvl));
            float prevWaves = waves;
            float life = LerpLevel(lifetimeLvl1, lifetimeLvl10, lvl) * 6;
            float prevLife = life;
            float stunDur = LerpLevel(stunLvl1, stunLvl10, lvl);
            float prevStunDur = stunDur;
            float kb = LerpLevel(knockbackLvl1, knockbackLvl10, lvl);
            float prevKb = kb;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevCd = Scriptable.GetCooldown(prevLvl);
                prevWaves = Mathf.FloorToInt(LerpLevel(wavesLvl1, wavesLvl10, prevLvl));
                prevLife = LerpLevel(lifetimeLvl1, lifetimeLvl10, prevLvl) * 6;
                prevStunDur = LerpLevel(stunLvl1, stunLvl10, prevLvl);
                prevKb = LerpLevel(knockbackLvl1, knockbackLvl10, prevLvl);
            }
            return new object[]
            {
                cd,          waves,             life,            stunDur,               kb,
                cd - prevCd, waves - prevWaves, life - prevLife, stunDur - prevStunDur, kb - prevKb,
            };
        }
    }
}