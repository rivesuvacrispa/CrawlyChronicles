﻿using System.Collections;
using Player;
using UnityEngine;

namespace Scripts.Gameplay.Bosses.Centipede
{
    public class CentipedeAttack : MonoBehaviour
    {
        private new Collider2D collider;

        private void Awake() => collider = GetComponent<Collider2D>();

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (PlayerManager.Instance.Damage(
                    CentipedeDefinitions.AttackDamage,
                    transform.position,
                    CentipedeDefinitions.Knockback) == 0)
            {
                PlayerManager.Instance.Knockback((Vector2) transform.position + Random.insideUnitCircle.normalized,
                    CentipedeDefinitions.Knockback);
            }
            if(gameObject.activeInHierarchy) StartCoroutine(DelayRoutine());
        }

        private IEnumerator DelayRoutine()
        {
            collider.enabled = false;
            yield return new WaitForSeconds(5f);
            collider.enabled = true;
        }
        
    }
}