using UnityEngine;

namespace Util.Components
{
    public class OrphanizeTransform : MonoBehaviour
    {
        private void Awake()
        {
            transform.SetParent(null);
        }
    }
}