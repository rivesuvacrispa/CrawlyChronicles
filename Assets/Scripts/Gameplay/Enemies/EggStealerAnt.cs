using Gameplay.AI;
using Gameplay.Food;
using UnityEngine;

namespace Gameplay.Enemies
{
    public class EggStealerAnt : Enemy
    {
        [SerializeField] private SpriteRenderer eggSpriteRenderer;

        private bool isHoldingEgg;

        protected override void Start()
        {
            eggSpriteRenderer.enabled = false;
            base.Start();
        }

        public override void OnMapEntered()
        {
            stateController.SetState(AIState.Wander);
        }

        public override void OnPlayerLocated()
        {
            stateController.SetState(AIState.Follow);
        }

        public override void OnEggsLocated(EggBed eggBed)
        {
            stateController.SetState(AIState.Follow, eggBed.gameObject, (go) =>
            {
                if(go is null)
                {
                    stateController.SetState(AIState.Wander);
                    return;
                }
                eggBed.RemoveOne();
                isHoldingEgg = true;
                eggSpriteRenderer.enabled = true;
                stateController.SetState(AIState.Flee);
            });
        }

        public override void OnFoodLocated(FoodBed foodBed)
        {
        }

        public override void OnDamageTaken()
        {
            if(isHoldingEgg) DropEgg();
        }

        private void DropEgg()
        {
            eggSpriteRenderer.enabled = false;
            isHoldingEgg = false;
        }
    }
}