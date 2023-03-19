using Gameplay.Food;

namespace Gameplay.Enemies
{
    public class AntWarrior : Enemy
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

        public override void OnFoodLocated(Foodbed foodBed)
        {
        }

        protected override void OnDamageTaken()
        {
        }
    }
}