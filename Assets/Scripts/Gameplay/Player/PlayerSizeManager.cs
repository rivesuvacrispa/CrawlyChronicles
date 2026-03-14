using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerSizeManager : MonoBehaviour
    {
        private static PlayerSizeManager instance;
        
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float initialMass = 0.175f;

        public static float CurrentSize { get; private set; } = 1f;

        public delegate void PlayerSizeEvent(float size);
        public static event PlayerSizeEvent OnSizeChanged;


        private PlayerSizeManager() => instance = this;
        public static void SetSize(float size)
        {
            instance.SetInstanceSize(size);
            OnSizeChanged?.Invoke(CurrentSize);
        }

        private void SetInstanceSize(float size)
        {
            CurrentSize = Mathf.Clamp(size, 0.5f, 2f);
            transform.localScale = Vector3.one * size;
            rb.mass = initialMass * size;
        }
    }
}