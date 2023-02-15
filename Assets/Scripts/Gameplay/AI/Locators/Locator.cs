using UnityEngine;

namespace Gameplay.AI.Locators
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Locator : MonoBehaviour
    {
        public delegate void LocatorEvent(ILocatorTarget target);

        public event LocatorEvent OnTargetLocated;

        public void SetRadius(float radius) => GetComponent<CircleCollider2D>().radius = radius;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent(out ILocatorTarget target)) 
                OnTargetLocated?.Invoke(target);
        }
    }
}