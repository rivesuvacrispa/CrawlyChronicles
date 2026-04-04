using Gameplay.Genes;
using UnityEngine;

namespace UI.Elements
{
    public class MutationSlotGroup : MonoBehaviour
    {
        private MutationSlotText[] texts;

        private void Awake()
        {
            texts = GetComponents<MutationSlotText>();
        }

        public void UpdateCanFit(TrioGene current, TrioGene max)
        {
            foreach (MutationSlotText text in texts)
            {
                text.UpdateCanFit(current, max);
            }
        }
    }
}