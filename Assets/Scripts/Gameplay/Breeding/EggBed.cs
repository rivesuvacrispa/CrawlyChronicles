using System.Collections.Generic;
using Definitions;
using GameCycle;
using Gameplay.AI.Locators;
using Gameplay.Interaction;
using Genes;
using UI;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EggBed : MonoBehaviour, INotificationProvider, ILocatorTarget, IContinuouslyInteractable
    {
        [SerializeField] private List<Egg> storedEggs = new();
        
        private SpriteRenderer spriteRenderer;

        public int EggsAmount => storedEggs.Count;
        public Egg GetEgg(int index) => storedEggs[index];
        
        private enum InteractionState
        {
            Eat,
            ReturnEgg,
        }
        
        
        
        private void Awake()
        {
            MainMenu.OnResetRequested += OnResetRequested;
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
            RespawnManager.OnEggCollectionRequested -= OnEggBedsCollectionRequested;
            OnProviderDestroy?.Invoke();
            MainMenu.OnResetRequested -= OnResetRequested;
        }

        // Should be called only when instantiating because it does not update total eggs amount
        public void SetEggs(IEnumerable<Egg> eggs)
        {
            storedEggs.AddRange(eggs);
            UpdateAmount();
        }

        private void AddEgg(Egg egg)
        {
            storedEggs.Add(egg);
            BreedingManager.Instance.AddTotalEggsAmount(1);
            UpdateAmount();
        }

        public void RemoveParticular(Egg egg)
        {
            storedEggs.Remove(egg);
            BreedingManager.Instance.AddTotalEggsAmount(-1);
            if (EggsAmount <= 0)
                Destroy(gameObject);
            else
                UpdateAmount();
        }
        
        public bool RemoveOne(out Egg egg)
        {
            egg = null;
            if (EggsAmount <= 0) return false;
            
            egg = storedEggs[Random.Range(0, EggsAmount)];
            storedEggs.Remove(egg);
            BreedingManager.Instance.AddTotalEggsAmount(-1);
            
            if (EggsAmount <= 0)
                Destroy(gameObject);
            else
                UpdateAmount();

            return true;
        }

        private void UpdateAmount()
        {
            spriteRenderer.sprite = GlobalDefinitions.GetEggsBedSprite(EggsAmount);
            OnDataUpdate?.Invoke();
        }

        private void Eat()
        {
            RemoveOne(out var egg);
            BreedingManager.Instance.AddFood();
            float genesAmount = Random.value * 0.05f;
            TrioGene drop = egg.Genes.Multiply(genesAmount);
            Vector3 pos = transform.position;
            GlobalDefinitions.CreateEggSquash(pos + (Vector3) Random.insideUnitCircle.normalized * 0.25f);
            GlobalDefinitions.DropGenesRandomly(pos, GeneType.Aggressive, drop.Aggressive);
            GlobalDefinitions.DropGenesRandomly(pos, GeneType.Defensive, drop.Defensive);
            GlobalDefinitions.DropGenesRandomly(pos, GeneType.Neutral, drop.Universal);
        }
        


        private void OnEggBedsCollectionRequested(List<EggBed> eggBeds) => eggBeds.Add(this);
        private void OnResetRequested() => Destroy(gameObject);

        

        
        // INotificationProvider 
        public event INotificationProvider.NotificationProviderEvent OnDataUpdate;
        public event INotificationProvider.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public string NotificationText => EggsAmount.ToString();


        
        // IContinuouslyInteractable
        private InteractionState interactionState;
        
        public void Interact()
        {
            if(interactionState == InteractionState.ReturnEgg)
            {
                AddEgg(Player.PlayerManager.Instance.HoldingEgg);
                StatRecorder.eggsLost--;
                Player.PlayerManager.Instance.RemoveEgg();
            }
            else Eat();
        }

        public bool CanInteract()
        {
            if (Player.PlayerManager.Instance.IsHoldingEgg)
            {
                if (EggsAmount >= 12) return false;
                interactionState = InteractionState.ReturnEgg;
            }
            else interactionState = InteractionState.Eat;

            return true;
        }

        public float PopupDistance => 0.75f;
        public string ActionTitle => interactionState == InteractionState.ReturnEgg ? "Return egg" : "Eat";
        Vector3 IInteractable.Position => transform.position;
        public void OnInteractionStart()
        {
        }

        public void OnInteractionStop()
        {
        }

        public float InteractionTime => interactionState == InteractionState.ReturnEgg ? 0 : 1f;
    }
}