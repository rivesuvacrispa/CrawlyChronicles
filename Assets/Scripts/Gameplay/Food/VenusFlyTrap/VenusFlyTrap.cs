using UnityEngine;

namespace Gameplay.Food.VenusFlyTrap
{
    public class VenusFlyTrap : Foodbed
    {
        private static bool existing;

        [SerializeField] private GameObject flowerGO;

        private int catchedTimes;
        
        public void Catch()
        {
            catchedTimes++;
            if (catchedTimes >= 6) flowerGO.SetActive(true);
        }

        protected override void Start()
        {
            base.Start();
            existing = true;
        }

        protected override void OnEatenByPlayer()
        {
        }

        public override bool CanSpawn(float random) => !existing && random < 1 / 3f;
        protected override void OnDestroy()
        {
            base.OnDestroy();
            existing = false;
        }

        public override bool CanInteract() => catchedTimes >= 6;
        protected override bool CreateNotification => false;
    }
}