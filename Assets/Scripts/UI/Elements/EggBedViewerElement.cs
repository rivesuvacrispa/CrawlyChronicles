using GameCycle;
using Gameplay.Breeding;
using UnityEngine;

namespace UI.Elements
{
    public class EggBedViewerElement : MonoBehaviour
    {
        [SerializeField] private GeneDisplay display;
        [SerializeField] private RespawnManager respawnManager;

        private Egg storedEgg;

        public void Click()
        {
            respawnManager.SelectEggToRespawn(storedEgg);
        }

        public void SetEgg(Egg egg)
        {
            storedEgg = egg;
            display.UpdateTrioText(egg.Genes);
        }
    }
}