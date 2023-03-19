using UnityEngine;

namespace Gameplay.Food
{
    public class FoodSpawnPoint : MonoBehaviour
    {
        public bool IsEmpty => foodBed is null;

        private Foodbed foodBed;

        public void Spawn(Foodbed toSpawn)
        {
            foodBed = Instantiate(toSpawn, transform);
            foodBed.FoodSpawnPoint = this;
        }

        public void Remove() => foodBed = null;
    }
}