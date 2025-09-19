using UnityEngine;

namespace Camera
{
    public class UICamera : MonoBehaviour
    {
        public static UnityEngine.Camera Camera { get; private set; }
        
        private void Awake() => Camera = GetComponent<UnityEngine.Camera>();
    }
}