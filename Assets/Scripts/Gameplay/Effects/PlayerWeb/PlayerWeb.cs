using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Enemies;
using Gameplay.Mutations.Active;
using Gameplay.Mutations.EntityEffects.Poison;
using Gameplay.Mutations.EntityEffects.Slow;
using Gameplay.Mutations.EntityEffects.Stat;
using Gameplay.Player;
using Pooling;
using UI.Menus;
using UnityEngine;
using Util;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Effects.PlayerWeb
{
    public class PlayerWeb : Poolable
    {
        [SerializeField] private Ease animationEase;
        [SerializeField, Range(0, 1)] private float animationDuration;
        [SerializeField, Range(0, 0.5f)] private float animationAffectSize = 0.05f;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D col;
        
        private MaterialPropertyBlock mpb;
        private Tween animTween;
        private float currentAnim;
        private static readonly int Size = Shader.PropertyToID("_Zoom");
        private float nextTriggerAllowedTime;
        private const float TRIGGER_INTERVAL = .25f;


        
        private void Awake()
        {
            mpb = new MaterialPropertyBlock();
            currentAnim = 0f;
            MainMenu.OnResetRequested += OnResetRequested;
        }

        private void OnDestroy() => MainMenu.OnResetRequested -= OnResetRequested;

        private void OnResetRequested() => Pool();

        private void OnTriggerEnter2D(Collider2D c)
        {
            TryApplyEffect(c);
            TryPlayAnimation();
        }
        
        private void OnTriggerStay2D(Collider2D c)
        {
            // Throttle trigger stay to 4 calls per second
            if (Time.fixedTime < nextTriggerAllowedTime) return;
            nextTriggerAllowedTime = Time.fixedTime + TRIGGER_INTERVAL;
            
            TryApplyEffect(c);
            if (Random.value > 0.025f) return;
            TryPlayAnimation();
        }

        private void OnTriggerExit2D(Collider2D c)
        {
            if (animTween is not null && animTween.IsPlaying()) return;

            animTween?.Kill();
            animTween = AnimateWobble();
        }

        private void TryPlayAnimation()
        {
            if (animTween is not null && animTween.IsPlaying()) return;
            
            animTween?.Kill();
            animTween = AnimateWobble();
        }

        private void TryApplyEffect(Collider2D c)
        {
            if (c.TryGetComponent(out IEffectAffectable affectable))
            {
                if (affectable is Enemy) 
                    affectable.AddEffect<SlowEntityEffect>(SpinneretGlands.DebuffEffectData);
                else if (affectable.Equals(PlayerManager.Instance)) 
                    PlayerManager.Instance.EffectController.AddEffect<PlayerWebEffect>(SpinneretGlands.BuffEffectData);
            }
        }
        
        private Tween AnimateWobble()
        {
            return DOTween.Sequence()
                .Append(PropertiesTween(-1, animationDuration / 4f))
                .Append(PropertiesTween(1, animationDuration / 2f))
                .Append(PropertiesTween(0, animationDuration / 4f))
                .SetEase(animationEase);
        }

        private Tween PropertiesTween(float endValue, float duration)
        {
            return DOTween.To(
                () => currentAnim,
                f =>
                {
                    currentAnim = f;
                    mpb.SetFloat(Size, 1 + f * animationAffectSize);
                    spriteRenderer.SetPropertyBlock(mpb);
                },
                endValue,
                duration);
        }

        public void PoolWithAnimation()
        {
            spriteRenderer.DOColor(Color.white.WithAlpha(0f), 0.5f).OnComplete(Pool);
        }

        public override bool OnTakenFromPool(object data)
        {
            // transform.localScale = Vector3.one * 2f;
            col.enabled = false;
            spriteRenderer.color = Color.white.WithAlpha(0f);
            spriteRenderer.DOColor(Color.white, 0.5f).OnComplete(() =>
            {
                col.enabled = true;
            });
            
            return base.OnTakenFromPool(data);
        }

 
    }
}