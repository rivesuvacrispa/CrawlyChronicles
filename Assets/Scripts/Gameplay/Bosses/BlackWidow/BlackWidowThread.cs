using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Util;

namespace Gameplay.Bosses.BlackWidow
{
    public class BlackWidowThread : MonoBehaviour
    {
        [SerializeField] private float decayTime = 0f;
        [SerializeField] private float appearTime = 1f;
        [SerializeField] private LineRenderer previewLine;
        [SerializeField] private LineRenderer part1;
        [SerializeField] private LineRenderer part2;
        [SerializeField] private new Collider2D collider;

        private bool hasDecayTime => decayTime > 0;
        
        
        private void Start()
        {
            SpawnTask().Forget();
        }

        private async UniTask SpawnTask()
        {
            await AnimateAppear();
            if (hasDecayTime) await AnimateDecay();
        }

        
        private async UniTask AnimateAppear()
        {
            Color c1 = Color.white.WithAlpha(0.4f);
            Color c2 = Color.white.WithAlpha(0f);
            await previewLine.DOColor(new Color2(c1, c1), new Color2(c2, c2), appearTime)
                .AsyncWaitForCompletion();

            previewLine.enabled = false;
            part1.enabled = true;
            part2.enabled = true;
            collider.enabled = true;
        }

        private async UniTask AnimateDecay()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(decayTime));

            Color c1 = Color.red;
            Color c2 = Color.red.WithAlpha(0f);
            
            previewLine.enabled = true;
            part1.enabled = false;
            part2.enabled = false;
            collider.enabled = false;
            previewLine.startColor = c1;
            previewLine.endColor = c1;

            await previewLine.DOColor(new Color2(c1, c1), new Color2(c2, c2), .35f)
                .AsyncWaitForCompletion();
            
            Destroy(gameObject);
        }

        public async UniTaskVoid DieOnHit(Vector3 contactPoint)
        {
            if (hasDecayTime) return;
            
            transform.position = contactPoint;

            collider.enabled = false;
            await UniTask.WhenAll(
                AnimatePart(part1, 1, -30f, 2f), 
                AnimatePart(part2, 0, 30f, 2f));
            Destroy(gameObject);
        }
        
        private async UniTask AnimatePart(LineRenderer line, int positionIndex, float endValue, float duration)
        {
            await DOTween.To(
                    () => line.GetPosition(positionIndex).y, 
                    pos => line.SetPosition(positionIndex, new Vector3(0, pos, 0)), 
                    endValue, duration)
                .AsyncWaitForKill();
        }

        public Vector3 FindClosestPointOnLine(Vector3 point)
        {
            Vector3 start = part1.transform.TransformPoint(part1.GetPosition(0));
            Vector3 end = part2.transform.TransformPoint(part2.GetPosition(1));

            Vector3 line = (end - start).normalized;
            if (line.sqrMagnitude == 0) return start;

            float t = Vector3.Dot(point - start, line) / Vector3.Dot(line, line);
            return start + t * line;
        }
    }
}