using GameCycle;
using Gameplay;
using UnityEngine;

namespace UI
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