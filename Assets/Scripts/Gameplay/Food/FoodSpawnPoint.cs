using UnityEngine;

namespace Gameplay.Food
{
    public class FoodSpawnPoint : MonoBehaviour
    {
        public bool IsEmpty => instantiatedGO is null;

        private GameObject instantiatedGO;

        public void Spawn(GameObject toSpawn)
        {
            instantiatedGO = Instantiate(toSpawn, transform);
        }

        public void Remove()
        {
            Destroy(instantiatedGO);
            instantiatedGO = null;
        }
    }
}