using UnityEngine;

namespace Util.Components
{
    public class AnimationHelper : MonoBehaviour
    {
        public void Disable() => gameObject.SetActive(false);
        public void Destroy() => Destroy(gameObject);
    }
}