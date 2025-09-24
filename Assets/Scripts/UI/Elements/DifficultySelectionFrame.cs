using System;
using System.Collections;
using UnityEngine;

namespace UI.Elements
{
    public class DifficultySelectionFrame : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float spacing;
        
        private float targetX;

        public void SetTargetX(int position) => targetX = spacing * position;

        private void OnEnable() => StartCoroutine(FollowRoutine());

        private IEnumerator FollowRoutine()
        {
            while (enabled)
            {
                Vector3 pos = transform.localPosition;
                float diff = Mathf.Lerp(0.1f, 1f, Math.Abs(pos.x - targetX) / spacing);
                transform.localPosition = new Vector3(
                    Mathf.MoveTowards(pos.x, targetX, speed * diff * Time.unscaledDeltaTime), pos.y, pos.z);
                yield return new WaitForSecondsRealtime(0f);
            }
        }
    }
}