using UnityEngine;

namespace Gameplay.AI.Locators
{
    [RequireComponent(typeof(Collider2D))]
    public class Locator : MonoBehaviour
    {
        public delegate void LocatorEvent(ILocatorTarget target);

        public event LocatorEvent OnTargetLocated;

        public void SetRadius(float radius)
        {
            if(TryGetComponent(out CircleCollider2D col)) col.radius = radius;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent(out ILocatorTarget target)) 
                OnTargetLocated?.Invoke(target);
        }
    }
}