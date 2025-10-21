using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Player;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Gameplay.Bosses.BlackWidow
{
    public class BlackWidowAnimatorHanging : MonoBehaviour
    {
        [SerializeField] private Transform spriteTransform;
        [SerializeField] private SpriteRenderer shadowSprite;
        [SerializeField] private float animationDuration = 3f;
        [SerializeField] private Ease animationEase;
        [SerializeField] private float followSpeed = 2f;
        [SerializeField] private new ParticleSystem particleSystem;


        private float currentHeightTarget;
        private float CurrentHeight => spriteTransform.localPosition.y;
        private bool hanging;
        private float durationPerHeight;
        private const float MIN_SHADOW_TRANSPARENCY = 0.45f;
        private const float MAX_SHADOW_HEIGHT = 5f;
        private readonly Vector3 shadowRatio = new Vector3(1f, 0.5f, 1f);
        private Vector3 hangPivot;

        private void Awake()
        {
            durationPerHeight = 3f / 20f;
            SetAtTop();
        }

        public void PlayParticles() => particleSystem.Play();

        public void HangAtHeight(float target)
        {
            HangAtHeightAsync(target).Forget();
        }

        private void Update()
        {
            if (!hanging) return;

            Transform parentTransform = transform.parent.transform;
            Vector3 currentPos = parentTransform.position;
            Vector3 targetPos = PlayerManager.Instance.transform.position + hangPivot;
            parentTransform.position = Vector3.MoveTowards(currentPos, targetPos, 
                followSpeed * Time.deltaTime * (targetPos - currentPos).sqrMagnitude / 9f);
        }

        public void SetRandomHangPivot()
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(4f, 6f);
            hangPivot = new Vector3(randomOffset.x, 0, randomOffset.y);
        }

        public async UniTask HangAtHeightAsync(float target)
        {
            target = Mathf.Clamp(target, 0, 20);
            if (Mathf.Abs(CurrentHeight - target) <= Mathf.Epsilon) return;
            
            if (!hanging)
            {
                SetRandomHangPivot();
            }

            currentHeightTarget = target;
            hanging = true;
            
            await PlayHeightChange();
        }

        private async UniTask PlayHeightChange()
        {
            float heightChange = currentHeightTarget - CurrentHeight;
            float duration = Mathf.Abs(heightChange) * durationPerHeight;
            
            var sequence = DOTween.Sequence()
                .Insert(0, spriteTransform.DOLocalMoveY(currentHeightTarget, duration)
                    .SetEase(animationEase));

            if (currentHeightTarget <= MAX_SHADOW_HEIGHT)
            {
                float shadowT = currentHeightTarget / MAX_SHADOW_HEIGHT;
                Color shadowColorAtTarget = Color.black.WithAlpha(
                    Mathf.Lerp(MIN_SHADOW_TRANSPARENCY, 0f, shadowT));
                Vector3 shadowRatioAtTarget = Vector3.Lerp(shadowRatio, shadowRatio * 4, shadowT);
                
                sequence
                    .Insert(0, shadowSprite.DOColor(shadowColorAtTarget, duration)
                        .SetEase(animationEase))
                    .Insert(0, shadowSprite.transform.DOScale(shadowRatioAtTarget, duration));
            }
            
            await sequence.AsyncWaitForKill();
        }


        private void SetAtTop()
        {
            spriteTransform.localPosition = new Vector3(0, 20, 0);
            shadowSprite.color = Color.black.WithAlpha(0f);
            shadowSprite.transform.localScale = shadowRatio * 4f;
        }

        private void SetAtBottom()
        {
            spriteTransform.localPosition = new Vector3(0, 0, 0);
            shadowSprite.color = Color.black.WithAlpha(MIN_SHADOW_TRANSPARENCY);
            shadowSprite.transform.localScale = shadowRatio;
        }
        
        public async UniTask PlayToBottom()
        {
            await HangAtHeightAsync(0f);
            hanging = false;
        }

        public async UniTask PlayToTop()
        {
            SetAtBottom();
            
            var sequence = DOTween.Sequence()
                .Insert(0, spriteTransform.DOLocalMoveY(20, animationDuration)
                    .SetEase(animationEase))
                .Insert(0, shadowSprite.DOColor(Color.black.WithAlpha(0f), animationDuration)
                    .SetEase(animationEase))
                .Insert(0, shadowSprite.transform.DOScale(new Vector3(4, 2, 4), animationDuration));

            await sequence.AsyncWaitForKill();
        }
    }
}