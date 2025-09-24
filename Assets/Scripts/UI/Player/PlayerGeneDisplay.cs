using Gameplay.Breeding;
using UI.Elements;

namespace UI.Player
{
    public class PlayerGeneDisplay : GeneDisplay
    {
        private void OnEnable()
        {
            BreedingManager.OnTrioGeneChange += UpdateTrioText;
        }

        private void OnDisable()
        {
            BreedingManager.OnTrioGeneChange -= UpdateTrioText;
        }
    }
}