using Gameplay.AI;
using Gameplay.Food;

namespace Gameplay.Enemies
{
    public class AntDefender : Enemy
    {
        public override void OnMapEntered()
        {
            stateController.SetState(AIState.Wander);
        }

        public override void OnPlayerLocated()
        {
            AttackPlayer();
        }

        public override void OnEggsLocated(EggBed eggBed)
        {
        }

        public override void OnFoodLocated(Foodbed foodBed)
        {
        }

        protected override void OnDamageTaken()
        {
            AttackPlayer();
        }
    }
}