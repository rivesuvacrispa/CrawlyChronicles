using Genes;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Active mutation")]
    public class ActiveMutation : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private GeneType geneType;
        [SerializeField] private KeyCode keyCode;
        [SerializeField] private Color spriteColor;
        [SerializeField] private Sprite sprite;
        [SerializeField] private float cooldownLvl1;
        [SerializeField] private float cooldownLvl10;

        public float GetCooldown(int level) =>
            Mathf.Lerp(cooldownLvl1, cooldownLvl10, level / 9f);
        public string Name => name;
        public GeneType GeneType => geneType;
        public Color SpriteColor => spriteColor;
        public Sprite Sprite => sprite;
        public KeyCode KeyCode => keyCode;
    }
}