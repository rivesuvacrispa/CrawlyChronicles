 using Timeline;
using UnityEngine;

namespace Gameplay.Food.VenusFlyTrap
{
    public class VenusFlyTrap : Foodbed
    {
        private static bool existing;

        [SerializeField] private GameObject flowerGO;

        private int caughtTimes;
        
        public void Catch()
        {
            caughtTimes++;
            if (caughtTimes >= 6) flowerGO.SetActive(true);
        }

        protected override void Start()
        {
            base.Start();
            existing = true;
        }

        protected override void OnEatenByPlayer()
        {
        }

        public override bool CanSpawn(float random) => TimeManager.DayCounter > 1 && !existing && random < 1 / 3f;
        protected override void OnDestroy()
        {
            base.OnDestroy();
            existing = false;
        }

        public override bool CanInteract() => base.CanInteract() && caughtTimes >= 6;
        protected override bool CreateNotification => false;
    }
}