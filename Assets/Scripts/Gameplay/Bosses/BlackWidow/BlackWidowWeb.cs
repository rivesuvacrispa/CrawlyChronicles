using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Player;
using UnityEngine;
using Util;

namespace Gameplay.Bosses.BlackWidow
{
    public class BlackWidowWeb : MonoBehaviour
    {
        [SerializeField] private PlayerStats statsDebuff;
        [SerializeField] private float animationTime = 1f;

        private SpriteRenderer[] renderers;
        
        private void Awake()
        {
            renderers = GetComponentsInChildren<SpriteRenderer>();

            SetAllColor(Color.white.WithAlpha(0f));
        }

        private void Start()
        {
            Appear().Forget();
        }

        private void OnDestroy()
        {
            ReturnPlayerStats();
        }

        public async UniTask Appear()
        {
            foreach (SpriteRenderer spriteRenderer in renderers)
            {
                spriteRenderer.DOColor(Color.white.WithAlpha(0.2f), animationTime);
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(animationTime));
            RemovePlayerStats();
        }

        public async UniTask Die()
        {
            foreach (SpriteRenderer spriteRenderer in renderers)
            {
                spriteRenderer.DOColor(Color.white.WithAlpha(0f), animationTime);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(animationTime));
            Destroy(gameObject);
        }

        private void SetAllColor(Color c)
        {
            foreach (SpriteRenderer spriteRenderer in renderers)
            {
                spriteRenderer.color = c;
            }
        }
        
        private void RemovePlayerStats()
        {
            PlayerManager.Instance.AddStats(statsDebuff.Negated());
        }

        private void ReturnPlayerStats()
        {
            PlayerManager.Instance.AddStats(statsDebuff);
        }
    }
}