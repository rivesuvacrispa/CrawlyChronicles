using Gameplay.AI;
using Gameplay.Food;

namespace Gameplay.Enemies
{
    public class WarriorAnt : Enemy
    {
        public override void OnMapEntered()
        {
            stateController.SetState(AIState.Follow);
        }

        public override void OnPlayerLocated()
        {
        }

        public override void OnEggsLocated(EggBed eggBed)
        {
        }

        public override void OnFoodLocated(FoodBed foodBed)
        {
        }

        public override void OnDamageTaken()
        {
        }
    }
}