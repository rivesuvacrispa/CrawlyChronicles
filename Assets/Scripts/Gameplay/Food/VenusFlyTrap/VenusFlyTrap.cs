 using Timeline;
using UnityEngine;

namespace Gameplay.Food.VenusFlyTrap
{
    public class VenusFlyTrap : UniqueFoodbed
    {
        [SerializeField] private GameObject flowerGO;

        private int caughtTimes;
        
        public void Catch()
        {
            caughtTimes++;
            if (caughtTimes >= 6) flowerGO.SetActive(true);
        }
        
        protected override void OnEatenByPlayer()
        {
        }

        public override bool CanSpawn(float random) => base.CanSpawn(random) && TimeManager.DayCounter > 1 && random < 1 / 3f;

        public override bool CanInteract() => base.CanInteract() && caughtTimes >= 6;
        protected override bool CreateNotification => false;
    }
}