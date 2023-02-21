using UI;
using UnityEngine;

namespace Gameplay.Food
{
    public class FoodSpawnPoint : MonoBehaviour
    {
        public bool IsEmpty => foodBed is null;

        private FoodBed foodBed;

        public void Spawn(FoodBed toSpawn)
        {
            foodBed = Instantiate(toSpawn, transform);
            foodBed.FoodSpawnPoint = this;
        }

        public void Remove()
        {
            if(foodBed is not null)
                Destroy(foodBed.gameObject);
            foodBed = null;
        }
    }
}