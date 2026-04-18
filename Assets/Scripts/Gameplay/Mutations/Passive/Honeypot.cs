using Gameplay.AI.Locators;
using Gameplay.Food;
using Gameplay.Player;
using Scriptable;
using Unity.VisualScripting;
using UnityEngine;
using Util.Abilities;
using Util.Components;
using Util.Enums;

namespace Gameplay.Mutations.Passive
{
    public class Honeypot : BasicAbility
    {
        [Header("Refs")]
        [SerializeField] private GameObject spriteGO;
        [SerializeField] private float minSpriteScale = 0.6f;
        [SerializeField] private float maxSpriteScale = 2.0f;
        [SerializeField] private Scriptable.Character antHoneypot;
        [SerializeField] private Locator locator;
        [SerializeField] private GameObject pollenPrefab;
        [Header("Stats")]
        [SerializeField] private LevelConst maxHoney = new LevelConst(100f);
        [SerializeField] private LevelConst healthPerHoney = new LevelConst(0.5f);

        private int honeyConsumed = 0;
        private PlayerStats addedStats = PlayerStats.Zero;
        
        
        
        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                maxHoney.UseKey(LevelFieldKeys.MAX_HONEY),
                healthPerHoney.UseKey(LevelFieldKeys.HEALTH_PER_POLLINATION)
            };
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetHoney(0);
            HideOrShowSprite();
            SubEvents();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnsubEvents();
            RemoveStats();
        }

        private void UpdateSprite()
        {
            float max = maxHoney.Value == 0 ? 1 : maxHoney.Value;
            float scale = Mathf.Lerp(minSpriteScale, maxSpriteScale, honeyConsumed / max);
            spriteGO.transform.localScale = Vector3.one * scale;
        }

        private void SubEvents()
        {
            locator.OnTargetLocated += OnTargetLocated;
            CharacterManager.OnCharacterSelected += OnCharacterSelected;
        }

        private void UnsubEvents()
        {
            locator.OnTargetLocated -= OnTargetLocated;
            CharacterManager.OnCharacterSelected -= OnCharacterSelected;
        }

        private void OnCharacterSelected(Character selected)
        {
            HideOrShowSprite();
        }

        private void OnTargetLocated(ILocatorTarget target)
        {
            if (target is FoodObject foodObject && 
                foodObject.FoodType == FoodType.Plant &&
                !foodObject.TryGetComponent(out Pollen _))
            {
                Instantiate(pollenPrefab, foodObject.transform);
                foodObject.AddComponent<Pollen>();
                SetHoney(honeyConsumed + 1);
            }
        }

        private void SetHoney(int honey)
        {
            int honeyLevel = (int) Mathf.Clamp(honey, 0, maxHoney.Value);
            if (honeyLevel == honeyConsumed) return;

            honeyConsumed = honeyLevel;
            UpdateSprite();
            RemoveStats();

            addedStats = new PlayerStats(maxHealth: honeyConsumed * healthPerHoney.Value);
            PlayerManager.Instance.AddStats(addedStats);
        }

        private void RemoveStats()
        {
            if (!addedStats.Equals(PlayerStats.Zero))
                PlayerManager.Instance.AddStats(addedStats.Negated());
        }

        private void HideOrShowSprite()
        {
            spriteGO.SetActive(antHoneypot.Equals(CharacterManager.CurrentCharacter));
        }
    }
}