using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerSizeManager : MonoBehaviour
    {
        private static PlayerSizeManager instance;
        
        [SerializeField] private TrailRenderer attackTrailRenderer;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float initialMass = 0.175f;

        public static float CurrentSize { get; private set; } = 1f;



        private PlayerSizeManager() => instance = this;
        public static void SetSize(float size) => instance.SetInstanceSize(size);
        
        private void SetInstanceSize(float size)
        {
            CurrentSize = Mathf.Clamp(size, 0.5f, 2f);
            transform.localScale = Vector3.one * size;
            attackTrailRenderer.widthMultiplier = size;
            rb.mass = initialMass * size;
        }
    }
}