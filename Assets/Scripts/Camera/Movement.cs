using UnityEngine;

namespace Camera
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float followingSpeed;

        private void Update()
        {
            Vector3 move = Vector3.MoveTowards(transform.position, target.position, followingSpeed * Time.deltaTime);
            move.z = -10;
            transform.position = move;
        }
    }
}