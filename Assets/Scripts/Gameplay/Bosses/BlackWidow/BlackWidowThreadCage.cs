using UnityEngine;

namespace Gameplay.Bosses.BlackWidow
{
    public class BlackWidowThreadCage : MonoBehaviour
    {
        [SerializeField] private float speed = 2f;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float targetRadius;
        [SerializeField] private float rotationDiffFactor = 1f;
        [SerializeField] private float minNormRadius = 0.1f;

        public void SetTargetRadius(float t)
        {
            targetRadius = Mathf.Max(t, 0f);
        }

        private void Awake()
        {
            transform.position = new Vector3(15, 15, 0);
        }

        private void Update()
        {
            if (transform.childCount == 0) return;

            Transform firstChild = transform.GetChild(0);
            float currentRadius = firstChild.localPosition.magnitude;
            float diffForRotation = targetRadius - currentRadius;

            float norm = Mathf.Max(targetRadius, minNormRadius);

            float effectiveRotationSpeed = rotationSpeed * (1f + rotationDiffFactor * Mathf.Abs(diffForRotation) / norm);
            transform.Rotate(Vector3.forward, effectiveRotationSpeed * Time.deltaTime);

            foreach (Transform child in transform)
            {
                Vector3 localPos = child.localPosition;
                float currentRadiusChild = localPos.magnitude;

                float diff = targetRadius - currentRadiusChild;
                if (Mathf.Abs(diff) < 0.01f) continue;

                Vector3 direction = (currentRadiusChild > 0f) ? localPos.normalized : Vector3.right;

                float damping = speed / norm;
                float moveDistance = diff * damping * Time.deltaTime;

                moveDistance = Mathf.Clamp(moveDistance, -Mathf.Abs(diff), Mathf.Abs(diff));

                child.localPosition = localPos + direction * moveDistance;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, targetRadius);
        }
    }
}