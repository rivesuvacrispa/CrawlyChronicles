using System.Text;
using Player;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Gameplay.Abilities.Active
{
    public class SonicScream : ActiveAbility
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
        
        private void OnParticleCollision(GameObject other)
        {
            if (other.TryGetComponent(out IDamageableEnemy enemy))
                enemy.Damage(
                    0,
                    PlayerMovement.Position,
                    knockback,
                    stun,
                    Color.white);
        }
        
        public override void Activate() => particleSystem.Play();
        
        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            StringBuilder sb = new StringBuilder();

            float prevCd = 0;
            float waves = Mathf.FloorToInt(LerpLevel(wavesLvl1, wavesLvl10, lvl));
            float prevWaves = 0;
            float life = LerpLevel(lifetimeLvl1, lifetimeLvl10, lvl) * 6;
            float prevLife = 0;
            float stunDur = LerpLevel(stunLvl1, stunLvl10, lvl);
            float prevStunDur = 0;
            float kb = LerpLevel(knockbackLvl1, knockbackLvl10, lvl);
            float prevKb = 0;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevCd = Scriptable.GetCooldown(prevLvl);
                prevWaves = Mathf.FloorToInt(LerpLevel(wavesLvl1, wavesLvl10, prevLvl));
                prevLife = LerpLevel(lifetimeLvl1, lifetimeLvl10, prevLvl) * 6;
                prevStunDur = LerpLevel(stunLvl1, stunLvl10, prevLvl);
                prevKb = LerpLevel(knockbackLvl1, knockbackLvl10, prevLvl);
            }
            
            sb.AddAbilityLine("Cooldown", Scriptable.GetCooldown(lvl), prevCd, false, suffix: "s");
            sb.AddAbilityLine("Num of waves", waves, prevWaves);
            sb.AddAbilityLine("Scream radius", life, prevLife);
            sb.AddAbilityLine("Stun duration", stunDur, prevStunDur, suffix: "s");
            sb.AddAbilityLine("Knockback", kb, prevKb);
            
            return sb.ToString();
        }
    }
}