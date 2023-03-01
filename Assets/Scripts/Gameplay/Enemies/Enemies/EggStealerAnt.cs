using Definitions;
using GameCycle;
using Gameplay.AI;
using Gameplay.Food;
using UnityEngine;

namespace Gameplay.Enemies
{
    public class EggStealerAnt : Enemy
    {
        [SerializeField] private SpriteRenderer eggSpriteRenderer;

        private Egg holdingEgg;

        protected override void Start()
        {
            eggSpriteRenderer.enabled = false;
            base.Start();
        }

        public override void OnMapEntered()
        {
            if(BreedingManager.TotalEggsAmount == 0) AttackPlayer();
            else stateController.SetState(AIState.Wander);
        }

        public override void OnPlayerLocated()
        {
            AttackPlayer();
        }

        public override void OnEggsLocated(EggBed eggBed)
        {
            stateController.SetState(AIState.Follow, eggBed, () =>
            {
                if(eggBed.RemoveOne(out holdingEgg))
                {
                    StatRecorder.eggsLost++;
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
            if(holdingEgg is not null) DropEgg();
            AttackPlayer();
        }

        private void DropEgg()
        {
            eggSpriteRenderer.enabled = false;
            var egg = GlobalDefinitions.CreateEggDrop(holdingEgg).transform;
            egg.position = (Vector3) rb.position + transform.up * 0.35f;
            egg.rotation = Quaternion.Euler(0, 0, rb.rotation);
            holdingEgg = null;
        }
    }
}