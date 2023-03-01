using Gameplay.Food;

namespace Gameplay.Enemies
{
    public class WarriorAnt : Enemy
    {
        public override void OnMapEntered()
        {
            AttackPlayer();
        }

        public override void OnPlayerLocated()
        {
            AttackPlayer();
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