using System.Collections.Generic;
using Gameplay.Genes;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Mutations/Basic mutation")]
    public class BasicMutation : ScriptableObject
    {
        [SerializeField] private GeneType geneType;
        [SerializeField] private bool takesSlot = true;
        [SerializeField] private bool notUpgradeable = false;
        [SerializeField] private Color spriteColor;
        [SerializeField] private Sprite sprite;
        [SerializeField] private bool hasIncompatible;
        [SerializeField] private List<BasicMutation> incompatibleMutations;

        private static readonly TableReference TableReference = "Abilities_All";
        private TableEntryReference nameReference;
        private TableEntryReference descriptionReference;
        
        public GeneType GeneType => geneType;
        public Color SpriteColor => spriteColor;
        public Sprite Sprite => sprite;
        public string Name => LocalizationSettings.StringDatabase.GetLocalizedString(TableReference, nameReference);
        public string Description => LocalizationSettings.StringDatabase.GetLocalizedString(TableReference, descriptionReference);
        public bool HasIncompatible => hasIncompatible;
        public List<BasicMutation> IncompatibleMutations => incompatibleMutations;
        public bool TakesSlot => takesSlot;
        public bool NotUpgradeable => notUpgradeable;



        public string GetStatDescription(object[] arguments)
            => string.Empty; /*LocalizationSettings.StringDatabase
                .GetLocalizedString(TableReference, StatsReference, arguments);*/
        private void Init()
        {
            nameReference = $"{name}_Name";
            descriptionReference = $"{name}_Description";
        }

        private void Awake() => Init();
        private void OnValidate() => Init();
    }
}