using Cysharp.Threading.Tasks;
using Definitions;
using Gameplay.AI.Locators;
using Gameplay.Breeding;
using Gameplay.Genes;
using Gameplay.Interaction;
using UI.Menus;
using UnityEngine;
using Util.Components;
using Util.Enums;

namespace Gameplay.Food
{
    [RequireComponent(typeof(PopAnimator))]
    public abstract class FoodObject : MonoBehaviour, ILocatorTarget, IContinuouslyInteractable
    {
        private PopAnimator popAnimator;
        public int Amount { get; protected set; }
        private bool destructionInvoked;
        public abstract int StartAmount { get; }
        public abstract FoodType FoodType { get; }




        protected abstract void OnEaten();
        protected abstract void OnEatenByPlayer();

        protected virtual void Awake()
        {
            popAnimator = GetComponent<PopAnimator>();
            transform.localScale = Vector3.zero;
        }

        protected virtual void Start()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            Amount = StartAmount;
            popAnimator.PlayPopIn(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }
        
        private void OnResetRequested()
        {
            Destroy(gameObject);
        }
        
        public bool Eat()
        {
            if (Amount <= 0) return false;

            Amount--;
            if (Amount <= 0)
            {
                DespawnTask().Forget();
            }
            else
            {
                popAnimator.PlayPop(gameObject.GetCancellationTokenOnDestroy()).Forget();
                OnEaten();
            }

            return true;
        }

        private async UniTask DespawnTask()
        {
            await popAnimator.PlayPopOut(gameObject.GetCancellationTokenOnDestroy());
            Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            MainMenu.OnResetRequested -= OnResetRequested;
        }

        // IContinuouslyInteractable
        public abstract float PopupDistance { get; }
        // TODO: localize
        public string ActionTitle => "Eat";
        public Vector3 Position => transform.position;
        public float InteractionTime => 1f;

        public virtual bool CanInteract()
        {
            return Amount > 0;
        }

        public void Interact()
        {
            if (Eat())
            {
                if (!BreedingManager.Instance.AddFood())
                    GlobalDefinitions.DropGenesRandomly(Position, (GeneType)Random.Range(0, 3), 1, 0.4f);
                OnEatenByPlayer();
            }
        }

        public virtual void OnInteractionStart()
        {
        }

        public virtual void OnInteractionStop()
        {
        }
    }
}