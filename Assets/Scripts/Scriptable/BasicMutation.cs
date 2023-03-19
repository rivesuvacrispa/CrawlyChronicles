using Genes;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Mutations/Basic mutation")]
    public class BasicMutation : ScriptableObject
    {
        [SerializeField] private bool special;
        [SerializeField] private new string name;
        [SerializeField] private GeneType geneType;
        [SerializeField] private Color spriteColor;
        [SerializeField] private Sprite sprite;
        [SerializeField, TextArea] private string description;

        public bool Special => special;
        public GeneType GeneType => geneType;
        public Color SpriteColor => spriteColor;
        public Sprite Sprite => sprite;
        public string Name => name;

        public string Description => description;
    }
}