using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Mutations.Stats
{
    public class MuscleGrowth : StatsAbility
    {
        [SerializeField, Range(1f, 2f)] private float sizeLvl1;
        [SerializeField, Range(1f, 2f)] private float sizeLvl10;

        private float size;

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            size = LerpLevel(sizeLvl1, sizeLvl10, lvl);
            UpdateBodySize(size);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateBodySize(size);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UpdateBodySize(1f);
        }

        private void UpdateBodySize(float s)
        {
            PlayerManager.Instance.Transform.localScale = Vector3.one * s;
        }
    }
}