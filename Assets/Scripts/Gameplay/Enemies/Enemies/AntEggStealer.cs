using Definitions;
using Gameplay.AI;
using Gameplay.Breeding;
using Gameplay.Food;
using UnityEngine;

namespace Gameplay.Enemies.Enemies
{
    public class AntEggStealer : Enemy
    {
        [SerializeField] private SpriteRenderer eggSpriteRenderer;
        
        protected Egg holdingEgg;

        public delegate void AntEggStealerEvent();
        public static event AntEggStealerEvent OnEggStolen;
        
        
        
        public override void OnMapEntered()
        {
            if(BreedingManager.TotalEggsAmount == 0) AttackPlayer();
            else StateController.SetState(AIState.Wander);
        }

        public override void OnPlayerLocated()
        {
            AttackPlayer();
        }

        public override void OnEggsLocated(EggBed eggBed)
        {
            StateController.SetState(AIState.Follow, eggBed, () =>
            {
                if(eggBed.RemoveOne(out holdingEgg))
                {
                    OnEggStolen?.Invoke();
                    eggSpriteRenderer.enabled = true;
                    StateController.SetState(AIState.Flee);
                }
                else StateController.SetState(AIState.Wander);
            }, reachDistance: 0.5f);
        }

        public override void OnFoodLocated(Foodbed foodBed)
        {
        }

        protected override void DamageTaken()
        {
            if (holdingEgg is not null) DropEgg();
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