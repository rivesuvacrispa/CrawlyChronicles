using Genes;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Mutations/Basic mutation")]
    public class BasicMutation : ScriptableObject
    {
        [SerializeField] private bool special;
        [SerializeField] private GeneType geneType;
        [SerializeField] private Color spriteColor;
        [SerializeField] private Sprite sprite;

        private TableReference tableReference;
        private static readonly TableEntryReference NameReference = "AbilityName";
        private static readonly TableEntryReference DescriptionReference = "AbilityDescription"; 
        private static readonly TableEntryReference StatsReference = "AbilityStats";
        
        public bool Special => special;
        public GeneType GeneType => geneType;
        public Color SpriteColor => spriteColor;
        public Sprite Sprite => sprite;
        public string Name => LocalizationSettings.StringDatabase.GetLocalizedString(tableReference, NameReference);
        public string Description => LocalizationSettings.StringDatabase.GetLocalizedString(tableReference, DescriptionReference);
        
        
        
        public string GetStatDescription(object[] arguments) 
            => LocalizationSettings.StringDatabase
                .GetLocalizedString(tableReference, StatsReference, arguments);
        private void Init() => tableReference = $"Abilities_{name}";
        private void Awake() => Init();
        private void OnValidate() => Init();
    }
}