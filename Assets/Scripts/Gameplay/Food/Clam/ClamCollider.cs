using UnityEngine;

namespace Gameplay.Food.Clam
{
    public class ClamCollider : MonoBehaviour
    {
        [SerializeField] private Clam clam;
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            clam.OnTouch(col);
        }
    }
}