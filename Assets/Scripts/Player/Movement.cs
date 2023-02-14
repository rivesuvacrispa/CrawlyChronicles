using Camera;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotationSpeed;
        
        private Rigidbody2D rb;

        
        
        private void Awake() => rb = GetComponent<Rigidbody2D>();

        private void Update()
        {
            Vector2 mousePos = MainCamera.WorldMousePos;
            Vector2 rotateDirection = mousePos - rb.position;
            float angle = Mathf.Atan2(rotateDirection.y, rotateDirection.x) - Mathf.PI * 0.5f;
            float rotation = Quaternion.RotateTowards(
                Quaternion.Euler(0, 0, rb.rotation), 
                quaternion.Euler(0, 0, angle),
                rotationSpeed * Time.deltaTime).eulerAngles.z;
            rb.rotation = rotation;

            if(Input.GetMouseButton(0))
            {
                Vector2 moveDirection = (transform.up * moveSpeed * Time.deltaTime);
                rb.velocity = moveDirection;
            }
        }
    }
}