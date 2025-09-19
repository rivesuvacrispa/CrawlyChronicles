using UnityEngine;

namespace Gameplay.Bosses.Centipede
{
    public class CentipedeTail : MonoBehaviour
    {
        private static readonly int TailAnimHash = Animator.StringToHash("CentipedeTail");

        private void Awake()
        {
            CentipedeFragment fragment = GetComponent<CentipedeFragment>();
            fragment.PlayAnimation(TailAnimHash);
            fragment.SetTail();
        }
    }
}