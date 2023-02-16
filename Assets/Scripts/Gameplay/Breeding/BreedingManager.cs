using System.Collections.Generic;
using Definitions;
using Gameplay.Genetics;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class BreedingManager : MonoBehaviour
    {
        public static BreedingManager Instance { get; private set; }

        [SerializeField] private GeneConsumer geneConsumer;
        [Header("Eggs")]
        [SerializeField] private Text totalEggsText;
        [SerializeField] private EggBed eggBedPrefab;
        [Header("Food")]
        [SerializeField] private Text foodText;
        [SerializeField] private int breedingFoodRequirement;
        [Header("Genes")] 
        [SerializeField] private Text aggressiveGenesAmountText;
        [SerializeField] private Text defensiveGenesAmountText;
        [SerializeField] private Text universalGenesAmountText;

        private int totalEggsAmount;
        private int currentFoodAmount;
        private TrioGene trioGene = TrioGene.Zero;
        private readonly Text[] geneTexts = new Text[3];
        
        
        
        private void Awake()
        {
            geneTexts[0] = aggressiveGenesAmountText;
            geneTexts[1] = defensiveGenesAmountText;
            geneTexts[2] = universalGenesAmountText;
            Instance = this;
            totalEggsAmount = 0;
            currentFoodAmount = 0;
            UpdateTotalEggsText();
            UpdateFoodText();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.R) && currentFoodAmount >= breedingFoodRequirement) LayEggs(Player.Movement.Position);
        }

        public void AddGene(GeneType geneType)
        {
            geneConsumer.ConsumeGene(geneType);
            trioGene.AddGene(geneType);
            UpdateGeneText(geneType);
        }
        
        private void LayEggs(Vector2 position)
        {
            var bed = Instantiate(eggBedPrefab, GlobalDefinitions.GameObjectsTransform);
            int amount = Random.Range(1, 7);
            var eggs = new List<TrioGene>();
            while (amount > 0)
            {
                eggs.Add(TrioGene.One);
                amount--;
            }
            
            bed.AddEggs(eggs);
            bed.transform.position = position;
            currentFoodAmount -= breedingFoodRequirement;
            UpdateFoodText();
        }

        private void UpdateTotalEggsText()
        {
            totalEggsText.text = totalEggsAmount.ToString();
            totalEggsText.color = totalEggsAmount == 0 ? Color.red : Color.white;
        }
        
        private void UpdateFoodText()
        {
            foodText.text = $"{currentFoodAmount}/{breedingFoodRequirement}";
            foodText.color = currentFoodAmount >= breedingFoodRequirement ? Color.green : Color.white;
        }

        private void UpdateGeneText(GeneType geneType) => geneTexts[(int) geneType].text = trioGene.GetGene(geneType).ToString();
        
        public void AddTotalEggsAmount(int amount)
        {
            totalEggsAmount += amount;
            UpdateTotalEggsText();
        }

        public void AddFood()
        {
            currentFoodAmount++;
            UpdateFoodText();
        }
    }
}