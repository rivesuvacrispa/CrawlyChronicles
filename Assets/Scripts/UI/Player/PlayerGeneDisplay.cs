using Gameplay.Breeding;
using UI.Elements;

namespace UI.Player
{
    public class PlayerGeneDisplay : GeneDisplay
    {
        private void Awake()
        {
            BreedingManager.OnTrioGeneChange += UpdateTrioText;
        }

        private void OnDestroy()
        {
            BreedingManager.OnTrioGeneChange -= UpdateTrioText;
        }
    }
}