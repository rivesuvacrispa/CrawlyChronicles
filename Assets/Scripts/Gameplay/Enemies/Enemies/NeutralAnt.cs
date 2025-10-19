using System.Collections;
using Definitions;
using Gameplay.AI;
using Gameplay.Breeding;
using Gameplay.Food;
using Gameplay.Food.Foodbeds;
using Gameplay.Genes;
using Gameplay.Interaction;
using Timeline;
using UI;
using UI.Menus;
using UnityEngine;
using Util;

namespace Gameplay.Enemies.Enemies
{
    public class NeutralAnt : Enemy, IContinuouslyInteractable
    {
        [SerializeField] private ParticleSystem breedingParticles;
        [SerializeField] private Animator breedAnimator;

        private delegate void NeutralAntEvent(Vector2 position);
        private static event NeutralAntEvent OnNeutralDamaged;

        public delegate void NeutralAntInteractionEvent();

        public static event NeutralAntInteractionEvent OnInteractionStarted;
        public static event NeutralAntInteractionEvent OnInteractionEnded;

        private static readonly int BreedAnimHash = Animator.StringToHash("NeutralAntBodyBreeding");
        private static readonly int IdleAnimHash = Animator.StringToHash("NeutralAntBodyIdle");

        private Coroutine interestRoutine;
        private bool hungry = true;
        private bool aggressive;
        public bool CanBreed { get; set; } = true;

        [field:SerializeField] public TrioGene TrioGene { get; private set; } = TrioGene.Zero;

        
        
        protected override void Start()
        {
            SubEvents();
            int entropy = GlobalDefinitions.BreedingPartnersGeneEntropy;
            TrioGene = BreedingManager.Instance.TrioGene.Randomize(entropy);
            TrioGene.AddGene(((Scriptable.Enemies.NeutralAnt)scriptable).GeneType, Random.Range(2 * entropy, 4 * entropy));
            
            base.Start();
        }

        public override void OnMapEntered()
        {
            StateController.SetState(AIState.Wander);
        }

        public override void OnPlayerLocated()
        {
            if (aggressive)
                AttackPlayer();
            else
            {
                StopInterest();
                if(TimeManager.IsDay && CanBreed) interestRoutine = StartCoroutine(InterestRoutine());
            }
        }

        public override void OnEggsLocated(EggBed eggBed)
        {
        }

        protected override void OnStunEnd()
        {
            if(!TimeManager.IsDay) OnNightStart(0);
        }

        public override void OnFoodLocated(Foodbed foodBed)
        {
            if (!hungry || foodBed is Ghostcap) return;
            StateController.SetState(AIState.Follow, 
                followTarget: foodBed,
                () => {
                    if (foodBed.Eat())
                    {
                        GlobalDefinitions.CreateRandomGeneDrop(transform.position);
                        hungry = false;
                    }
                    
                    StartCoroutine(HungerRoutine());
                    StateController.SetState(AIState.Wander);
                });
        }

        protected override void OnDamageTaken()
        {
            OnNeutralDamaged?.Invoke(rb.position);
        }
        
        private IEnumerator InterestRoutine()
        {
            StateController.TakeMoveControl();
            animator.Play(Scriptable.IdleAnimHash);

            float t = 0;
            while (t < 2f)
            {
                rb.RotateTowardsPosition(Player.PlayerMovement.Position, 5);
                t += Time.deltaTime;
                yield return null;
            }
            
            StateController.ReturnMoveControl();
            interestRoutine = null;
            StateController.SetState(AIState.Wander);
            animator.Play(Scriptable.WalkAnimHash);
        }

        private void StopInterest()
        {
            if(interestRoutine is not null) StopCoroutine(interestRoutine);
            StateController.ReturnMoveControl();
            animator.Play(Scriptable.WalkAnimHash);
        }

        private IEnumerator HungerRoutine()
        {
            yield return new WaitForSeconds(10);
            hungry = true;
        }

        private void OnNightStart(int day)
        {
            StopInterest();
            if(hitbox.Dead)             
                animator.Play(Scriptable.IdleAnimHash);
            StateController.SetEtherial(true);
            CanBreed = false;
            StateController.SetState(AIState.Flee);
        }

        private void OnNeutralDamage(Vector2 pos)
        {
            if(Vector2.Distance(rb.position, pos) > 7.5f || StateController.CurrentState == AIState.Enter) return;
            StopInterest();
            aggressive = true;
            minimapIcon.color = Color.red;
            CanBreed = false;
            AttackPlayer();
            OnNeutralDamaged -= OnNeutralDamage;
        }
        
        private void SubEvents()
        {
            OnNeutralDamaged += OnNeutralDamage;
            TimeManager.OnNightStart += OnNightStart;
        }

        private void UnsubEvents()
        {
            OnNeutralDamaged -= OnNeutralDamage;
            TimeManager.OnNightStart -= OnNightStart;
        }

        protected override void OnDestroy()
        {
            UnsubEvents();
            base.OnDestroy();
        }


        // IInteractable
        public bool CanInteract() => CanBreed && BreedingManager.Instance.CanBreed;
        
        public void Interact() => BreedingMenu.OpenBreedingMenu(this);

        public void OnInteractionStart()
        {
            UnsubEvents();
            StateController.SetState(AIState.None);
            StopInterest();
            rb.RotateTowardsPosition(Player.PlayerMovement.Position, 360);
            breedAnimator.Play(BreedAnimHash);
            animator.Play(Scriptable.IdleAnimHash);
            breedingParticles.Play();
            OnInteractionStarted?.Invoke();
        }

        public void OnInteractionStop()
        {
            breedAnimator.Play(IdleAnimHash);
            StateController.SetState(AIState.Wander);
            breedingParticles.Stop();
            animator.Play(Scriptable.WalkAnimHash);
            OnInteractionEnded?.Invoke();
            if(TimeManager.IsDay) SubEvents();
            else OnNightStart(0);
            if(!CanBreed) StateController.SetState(AIState.Flee);
        }

        public float InteractionTime => 3f;
        public float PopupDistance => 0.75f;
        public string ActionTitle => "Breed";
        Vector3 IInteractable.Position => transform.position;
    }
}