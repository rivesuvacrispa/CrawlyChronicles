using UnityEngine;

namespace Gameplay.Player
{
    public class Phantom : MonoBehaviour
    {
        public static Transform Transform { get; private set; }
        
        public delegate void PhantomEvent();

        public static event PhantomEvent OnPhantomSpawn;
        public static event PhantomEvent OnPhantomDespawn;

        public Phantom()
        {
            Transform = transform;
            Debug.Log("Phantom set");
        }

        private void OnEnable() => OnPhantomSpawn?.Invoke();

        private void OnDisable() => OnPhantomDespawn?.Invoke();
    }
}