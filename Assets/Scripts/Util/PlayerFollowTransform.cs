using Gameplay.Player;
using UnityEngine;

namespace Util
{
    public class PlayerFollowTransform : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float maxDistance;

        private void Update()
        {
            Transform t = transform;
            Vector3 pos = t.position;
            Vector3 playerPos = PlayerPhysicsBody.Position;
            float distance = Mathf.Clamp01(Vector2.Distance(pos, playerPos) / maxDistance);
            t.position = Vector3.MoveTowards(pos, playerPos, Mathf.Lerp(speed, 10f, distance) * Time.deltaTime);
        }
    }
}