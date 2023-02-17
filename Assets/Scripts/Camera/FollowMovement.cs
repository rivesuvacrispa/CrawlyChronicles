using UnityEngine;

namespace Camera
{
    public class FollowMovement : MonoBehaviour
    {
        [SerializeField] private float followingSpeed;

        public Transform Target { get; set; }

        private void Start()
        {
            Target = Player.Movement.Transform;
        }

        private void Update()
        {
            Vector3 move = Vector3.MoveTowards(transform.position, Target.position, followingSpeed * Time.deltaTime);
            move.z = -10;
            transform.position = move;
        }

        public void UpdateUnscaled()
        {
            Vector3 move = Vector3.MoveTowards(transform.position, Target.position, followingSpeed * Time.unscaledDeltaTime);
            move.z = -10;
            transform.position = move;
        }
    }
}