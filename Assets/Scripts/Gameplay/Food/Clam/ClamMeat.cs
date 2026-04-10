using Gameplay.Breeding;
using Gameplay.Player;
using UI.Menus;

namespace Gameplay.Food.Clam
{
    public class ClamMeat : FoodObject
    {
        public override int StartAmount => 1;
        protected override void OnEaten()
        {
            
        }

        protected override void OnEatenByPlayer()
        {
            MutationManager.Instance.MutateFromPlayer();
        }

        public override float PopupDistance => 0.75f;
    }
}