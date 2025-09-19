using Gameplay.Breeding;
using Player;
using Timeline;
using UI;

namespace Gameplay.Food.Foodbeds
{
    public class Ghostcap : Fungi
    {
        protected override void OnEatenByPlayer()
        {
            MutationMenu.Show(MutationTarget.Player, 
                new Egg(BreedingManager.Instance.TrioGene, AbilityController.GetMutationData()));
        }

        public override bool CanSpawn(float rnd) => !TimeManager.IsDay && rnd < 1 / 3f;
    }
}