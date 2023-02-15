using Definitions;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class BreedingManager : MonoBehaviour
    {
        public static BreedingManager Instance { get; private set; }

        [SerializeField] private Text totalEggsText;
        [SerializeField] private EggBed eggBedPrefab;

        private int totalEggsAmount;
        
        
        
        private void Awake()
        {
            Instance = this;
            totalEggsAmount = 0;
            UpdateTotalEggsText();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.E)) LayEggs(Player.Movement.Position);
        }
        
        private void LayEggs(Vector2 position)
        {
            var bed = Instantiate(eggBedPrefab, GlobalDefinitions.GameObjectsTransform);
            int amount = Random.Range(1, 7);
            bed.SetAmount(amount);
            bed.transform.position = position;
        }

        private void UpdateTotalEggsText()
        {
            totalEggsText.text = totalEggsAmount.ToString();
            totalEggsText.color = totalEggsAmount == 0 ? Color.red : Color.white;
        }
        
        public void AddTotalEggsAmount(int amount)
        {
            totalEggsAmount += amount;
            UpdateTotalEggsText();
        }
    }
}