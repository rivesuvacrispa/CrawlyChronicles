using System.Collections.Generic;
using Definitions;
using Gameplay.AI.Locators;
using Gameplay.Genetics;
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

        private int EggsAmount => eggs.Count;
        
        
        
        private void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();

        private void Start()
        {
            spriteRenderer.sprite = GlobalDefinitions.GetEggsBedSprite(EggsAmount);
            GlobalDefinitions.CreateNotification(this);
            BreedingManager.Instance.AddTotalEggsAmount(EggsAmount);
        }

        private void OnDestroy()
        {
            BreedingManager.Instance.AddTotalEggsAmount(-EggsAmount);
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
        
        
        
        // INotificationProvider 
        public event INotificationProvider.NotificationProviderEvent OnDataUpdate;
        public event INotificationProvider.NotificationProviderEvent OnProviderDestroy;
        public Vector2 Position => transform.position;
        public string NotificationText => EggsAmount.ToString();
        
        
        
        // IInteractable
        public void Interact()
        {
            AddEgg(Player.Manager.Instance.HoldingEgg);
            Player.Manager.Instance.RemoveEgg();
        }

        public bool CanInteract() => Player.Manager.Instance.IsHoldingEgg;
        public float InteractionTime => 0f;
        public float PopupDistance => 0.75f;
        public string ActionTitle => "Return egg";
        Vector3 IInteractable.Position => transform.position;
    }
}