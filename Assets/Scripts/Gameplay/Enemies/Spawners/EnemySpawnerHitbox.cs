﻿using System.Collections;
using Definitions;
using Gameplay.Player;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Gameplay.Enemies.Spawners
{
    [RequireComponent(typeof(Collider2D)),
     RequireComponent(typeof(DamageableEnemySpawner))]
    public class EnemySpawnerHitbox : MonoBehaviour
    {
        [SerializeField] private BodyPainter painter;


        private DamageableEnemySpawner spawner;
        private new Collider2D collider;

        public bool Enabled => collider.enabled;

        
        
        private void Awake()
        {
            collider = GetComponent<Collider2D>();
            spawner = GetComponent<DamageableEnemySpawner>();
        }

        private void OnTriggerEnter2D(Collider2D _) 
            => ((IDamageable) spawner).Damage(
                PlayerManager.PlayerStats.AttackDamage,
                default, 0, 0, default, false,
                effects: PlayerAttack.CurrentAttackEffects);
        
        public void Hit() => StartCoroutine(ImmunityRoutine());

        private IEnumerator ImmunityRoutine()
        { 
            collider.enabled = false;
            painter.FadeOut(GlobalDefinitions.EnemyImmunityDuration);
            yield return new WaitForSeconds(GlobalDefinitions.EnemyImmunityDuration);
            collider.enabled = true;
        }
    }
}