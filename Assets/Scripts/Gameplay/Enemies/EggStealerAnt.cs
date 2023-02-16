using Definitions;
using Gameplay.AI;
using Gameplay.Food;
using Gameplay.Genetics;
using UnityEngine;

namespace Gameplay.Enemies
{
    public class EggStealerAnt : Enemy
    {
        [SerializeField] private SpriteRenderer eggSpriteRenderer;

        private bool isHoldingEgg;
        private TrioGene holdingGene;

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
                if(eggBed.RemoveOne(out holdingGene))
                {
                    isHoldingEgg = true;
                    eggSpriteRenderer.enabled = true;
                    stateController.SetState(AIState.Flee);
                }
                else stateController.SetState(AIState.Wander);
            });
        }

        public override void OnFoodLocated(FoodBed foodBed)
        {
        }

        protected override void OnDamageTaken()
        {
            if(isHoldingEgg) DropEgg();
        }

        private void DropEgg()
        {
            eggSpriteRenderer.enabled = false;
            isHoldingEgg = false;
            var egg = GlobalDefinitions.CreateEgg(holdingGene).transform;
            egg.position = (Vector3) rb.position + transform.up * 0.35f;
            egg.rotation = Quaternion.Euler(0, 0, rb.rotation);
        }
    }
}