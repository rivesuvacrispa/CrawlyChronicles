using UnityEngine;

namespace Scripts.Gameplay.Bosses.Terrorwing
{
    public class TerrorwingAttack : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D col) => gameObject.SetActive(false);
    }
}