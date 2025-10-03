using Gameplay.AI.Locators;
using UnityEngine;

namespace Gameplay.Enemies.Enemies
{
    public class TermiteWorkerLocatorTarget : MonoBehaviour, ILocatorTarget
    {
        [SerializeField] private TermiteWorker termiteWorker;

        public TermiteWorker TermiteWorker => termiteWorker;
    }
}