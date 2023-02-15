using Gameplay.AI.Locators;
using UnityEngine;

namespace Gameplay.Food
{
    public abstract class FoodBed : MonoBehaviour, ILocatorTarget, IFoodBed
    {
        public string LocatorTargetName => "Food";
    }
}