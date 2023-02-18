using Gameplay.AI;
using Gameplay.Food;

namespace Gameplay.Enemies
{
    public class WarriorAnt : Enemy
    {
        public override void OnMapEntered()
        {
            stateController.SetState(AIState.Follow, 
                onTargetReach: o => BasicAttack(),
                reachDistance: 1.25f);
        }

        public override void OnPlayerLocated()
        {
            stateController.SetState(AIState.Follow, 
                onTargetReach: o => BasicAttack(),
                reachDistance: 1.25f);
        }

        public override void OnEggsLocated(EggBed eggBed)
        {
        }

        public override void OnFoodLocated(FoodBed foodBed)
        {
        }

        protected override void OnDamageTaken()
        {
        }
    }
}