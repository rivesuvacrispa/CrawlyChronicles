using Gameplay.AI.Locators;
using UnityEngine;

namespace Gameplay.Enemies.Enemies
{
    public class TermiteSwarmerLocatorTarget : MonoBehaviour, ILocatorTarget
    {
        [SerializeField] private TermiteSwarmer termiteSwarmer;

        public TermiteSwarmer TermiteSwarmer => termiteSwarmer;
    }
}