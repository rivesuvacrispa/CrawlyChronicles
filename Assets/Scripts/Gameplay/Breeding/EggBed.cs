using System.Collections.Generic;
using Definitions;
using GameCycle;
using Gameplay.AI.Locators;
using Genes;
using Gameplay.Interaction;
using UnityEngine;
using Util;

namespace Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EggBed : MonoBehaviour, INotificationProvider, ILocatorTarget, IInteractable
    {
        [SerializeField] private List<TrioGene> eggs = new();
        
        private SpriteRenderer spriteRenderer;

        public int EggsAmount => eggs.Count;
        public TrioGene GetEgg(int index) => eggs[index];
        
        
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            RespawnManager.OnEggCollectionRequested += OnEggBedsCollectionRequested;
        }

        private void Start()
        {
            spriteRenderer.sprite = GlobalDefinitions.GetEggsBedSprite(EggsAmount);
            GlobalDefinitions.CreateNotification(this);
            BreedingManager.Instance.AddTotalEggsAmount(EggsAmount);
        }

        private void OnDestroy()
        {
            BreedingManager.Instance.AddTotalEggsAmount(-EggsAmount);
            RespawnManager.OnEggCollectionRequested -= OnEggBedsCollectionRequested;
            OnProviderDestroy?.Invoke();
        }

        public void AddEggs(List<TrioGene> trioGenes)
        {
            eggs.AddRange(trioGenes);
            BreedingManager.Instance.AddTotalEggsAmount(trioGenes.Count);
            UpdateAmount();
        }

        private void AddEgg(TrioGene trioGene)
        {
            eggs.Add(trioGene);
            BreedingManager.Instance.AddTotalEggsAmount(1);
            UpdateAmount();
        }

        public bool RemoveOne(out TrioGene gene)
        {
            gene = default;
            if (EggsAmount <= 0) return false;
            
            gene = eggs[Random.Range(0, EggsAmount)];
            eggs.Remove(gene);

            if (EggsAmount <= 0)
                Destroy(gameObject);
            else
            {
                UpdateAmount();
                BreedingManager.Instance.AddTotalEggsAmount(-1);
            }

            return true;
        }

        private void UpdateAmount()
        {
            spriteRenderer.sprite = GlobalDefinitions.GetEggsBedSprite(EggsAmount);
            OnDataUpdate?.Invoke();
        }

        private void OnEggBedsCollectionRequested(List<EggBed> eggBeds) => eggBeds.Add(this);


        
        // INotificationProvider 
        public event INotificationProvider.NotificationProviderEvent OnDataUpdate;
        public event INotificationProvider.NotificationProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public string NotificationText => EggsAmount.ToString();


        // IInteractable


        public void Interact()
        {
            AddEgg(Player.Manager.Instance.HoldingEgg);
            Player.Manager.Instance.RemoveEgg();
        }

        public bool CanInteract() => Player.Manager.Instance.IsHoldingEgg && EggsAmount < 12;
        public float PopupDistance => 0.75f;
        public string ActionTitle => "Return egg";
        Vector3 IInteractable.Position => transform.position;
    }
}