using System;
using System.Collections;
using Definitions;
using Gameplay.AI.Locators;
using Gameplay.Breeding;
using Gameplay.Enemies;
using Gameplay.Food;
using Gameplay.Food.VenusFlyTrap;
using Gameplay.Map;
using Gameplay.Player;
using Hitboxes;
using Pathfinding;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.AI
{
    [RequireComponent(typeof(AIPath)),
     RequireComponent(typeof(AIDestinationSetter)),
     RequireComponent(typeof(Enemy))]
    public class AIStateController : MonoBehaviour
    {
        [SerializeField] private Locator locator;
        [SerializeField] private DamageableEnemyHitbox hitbox;
        [SerializeField] private Collider2D physicsCollider;

        [field: SerializeField] public AIState StartingState { get; set; } = AIState.Enter;

        protected Enemy enemy;
        private CallbackableAIPath aiPath;
        private AIDestinationSetter destinationSetter;
        private float defaultReachDistance;
        private ITransformProvider currentFollowTarget;
        
        public bool Etherial { get; private set; }
        private float movementSpeedMultiplier = 1f;
        private float rotationSpeedMultiplier = 1f;
        private bool locatorDismissed;
        
        public AIState CurrentState { get; private set; }
        public float CurrentMovementSpeed { get; private set; } = 1f;
        public float CurrentRotationSpeed { get; private set; } = 360f;

        public delegate void StateControllerEvent(AIStateController controller);
        public event StateControllerEvent OnBecomeEtherial;

        public float MovementSpeedMultiplier
        {
            get => movementSpeedMultiplier;
            set
            {
                movementSpeedMultiplier = value;
                UpdateMovementSpeed();
            }
        }
        
        public float RotationSpeedMultiplier
        {
            get => rotationSpeedMultiplier;
            set
            {
                rotationSpeedMultiplier = value;
                UpdateRotationSpeed();
            }
        }

        
        
        private void Awake()
        {
            enemy = GetComponent<Enemy>();
            aiPath = GetComponent<CallbackableAIPath>();
            destinationSetter = GetComponent<AIDestinationSetter>();

            locator.OnTargetLocated += OnLocatorTriggered;
            PlayerLocatorBody.OnDisabled += OnPlayerBecomeInvisible;
            aiPath.enabled = false;
            destinationSetter.enabled = false;
            defaultReachDistance = aiPath.endReachedDistance;
        }

        private void OnDestroy()
        {
            locator.OnTargetLocated -= OnLocatorTriggered;
            PlayerLocatorBody.OnDisabled -= OnPlayerBecomeInvisible;
            if(currentFollowTarget is not null)
                currentFollowTarget.OnProviderDestroy -= OnFollowTargetDestroy;
        }

        private void OnPlayerBecomeInvisible()
        {
            if (CurrentState is AIState.Follow && currentFollowTarget.Equals(PlayerManager.Instance))
                SetState(AIState.Wander);
        }

        private IEnumerator Start()
        {            
            yield return new WaitUntil(() => MapManager.AllSpawnPointsInitialized);
            SetState(StartingState);
            UpdateMovementSpeed();
            UpdateRotationSpeed();
            locator.SetRadius(enemy.Scriptable.LocatorRadius);
        }

        public void SetState(
            AIState newState, 
            ITransformProvider followTarget = null, 
            Action onTargetReach = null,
            float reachDistance = float.NaN)
        {
            if(newState == CurrentState) return;
            
            Debug.Log($"[{gameObject.name}] AI state changed to <{newState}>, path: [{(aiPath is null ? "None" : aiPath.GetHashCode())}]");
            
            CancelPath();
            
            switch (newState)
            {
                case AIState.Enter:
                    SetEnter();
                    break;
                case AIState.Exit:
                    SetExit();
                    break;
                case AIState.Wander:
                    SetWander();
                    break;
                case AIState.Follow:
                    StartCoroutine(SetFollow(followTarget, onTargetReach, reachDistance));
                    break;
                case AIState.None:
                    SetNone();
                    break;
                case AIState.Flee:
                    SetFlee();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
            
            CurrentState = newState;
            StartingState = newState;
        }

        protected virtual void SetEnter()
        {
            SetEtherial(true);
            SetDefaultReachDistance();
            DisableLocator();
            DoNotRepath();
            
            if (enemy.SpawnLocation is null) return;
            
            destinationSetter.enabled = false;
            aiPath.enabled = true;
            aiPath.SetPath(enemy.SpawnLocation.EnteringPath);
            aiPath.Callback = () =>
            {
                CancelCallback();
                SetEtherial(false);
                enemy.OnMapEntered();
            };
        }

        private void SetExit()
        {
            DisableLocator();
            AutoRepath();
            destinationSetter.enabled = false;
            aiPath.enabled = true;
            aiPath.destination = enemy.SpawnLocation.SpawnPosition;
            aiPath.SearchPath();
            aiPath.endReachedDistance = 1f;
            aiPath.Callback = () => Destroy(gameObject);
        }

        // TODO: why is this IEnumerator?
        private IEnumerator SetFollow(ITransformProvider target, Action onTargetReach, float reachDistance)
        {
            // yield return new WaitForEndOfFrame();
            aiPath.endReachedDistance = reachDistance.Equals(float.NaN) ? defaultReachDistance : reachDistance;
            target ??= PlayerManager.Instance;
            currentFollowTarget = target;
            DisableLocator();
            AutoRepath();
            CurrentMovementSpeed = 1;
            UpdateMovementSpeed();
            destinationSetter.enabled = true;
            aiPath.enabled = true;
            destinationSetter.target = target.Transform;
            target.OnProviderDestroy += OnFollowTargetDestroy;
            if (onTargetReach is not null) aiPath.Callback = onTargetReach;
            yield return null;
        }

        private void SetNone()
        {
            DisableLocator();
            DoNotRepath();
            destinationSetter.enabled = false;
            aiPath.enabled = false;
        }
        
        private void SetWander()
        {
            SetDefaultReachDistance();
            EnableLocator();
            AutoRepath();
            destinationSetter.enabled = false;
            aiPath.enabled = true;
            CurrentMovementSpeed = GlobalDefinitions.WanderingSpeedMultiplier;
            UpdateMovementSpeed();
            PickRandomDestination();
            aiPath.Callback = PickRandomDestination;
        }

        private void SetFlee()
        {
            CurrentMovementSpeed = GlobalDefinitions.FleeingSpeedMultiplier;
            UpdateMovementSpeed();
            SetState(AIState.Exit);
        }

        public void SetEtherial(bool isEtherial)
        {
            Debug.Log($"[{gameObject.name}] Set Etherial <{isEtherial}>");
            Etherial = isEtherial;
            physicsCollider.enabled = !isEtherial;
            if (isEtherial) hitbox.Disable();
            else hitbox.Enable();
            OnBecomeEtherial?.Invoke(this);
        }
        
        public void TakeMoveControl() => aiPath.canMove = false;

        public void ReturnMoveControl() => aiPath.canMove = true;

        

        // Utility
        protected bool TryGetRandomPointAround(Vector3 from, int radius, out Vector3 point)
        {
            point = default;
            if (AstarPath.active is null) return false;
            var startNode = AstarPath.active.GetNearest(from).node;
            var nodes = PathUtilities.BFS(startNode, radius);
            if(nodes.Count == 0) return false;
            point = PathUtilities.GetPointsOnNodes(nodes, 1)[0];
            return true;
        }
        
        private void PickRandomDestination()
        {
            if (TryGetRandomPointAround(transform.position, enemy.Scriptable.WanderingRadius, out Vector3 point))
            {
                aiPath.destination = point;
                aiPath.SearchPath();
            }
        }

        protected void DoNotRepath() => aiPath.autoRepath.mode = AutoRepathPolicy.Mode.Never;
        private void AutoRepath() => aiPath.autoRepath.mode = AutoRepathPolicy.Mode.Dynamic;
        private void UpdateMovementSpeed() => aiPath.maxSpeed = CurrentMovementSpeed * enemy.Scriptable.MovementSpeed * MovementSpeedMultiplier;
        private void UpdateRotationSpeed() => aiPath.rotationSpeed = CurrentRotationSpeed * RotationSpeedMultiplier;

        public void CancelCallback() => aiPath.Callback = null;
        
        private void CancelPath()
        {
            CancelCallback();
            DiscardFollowTarget();
            aiPath.SetPath(null);
        }

        private void OnLocatorTriggered(ILocatorTarget target)
        {
            switch (target)
            {
                case EggBed eggBed:
                    enemy.OnEggsLocated(eggBed);
                    break;
                case Foodbed foodBed:
                    enemy.OnFoodLocated(foodBed);
                    break;
                case PlayerLocatorBody:
                    enemy.OnPlayerLocated();
                    break;
            }
        }

        private void DiscardFollowTarget()
        {
            if (currentFollowTarget is null) return;
            currentFollowTarget.OnProviderDestroy -= OnFollowTargetDestroy;
            currentFollowTarget = null;
        }

        private void OnFollowTargetDestroy(IDestructionEventProvider provider) => SetState(AIState.Wander);

        protected void SetDefaultReachDistance() => aiPath.endReachedDistance = defaultReachDistance;
        protected void DisableLocator() => locator.gameObject.SetActive(false);
        private void EnableLocator()
        {
            if (!locatorDismissed)
                locator.gameObject.SetActive(true);
        }

        public void DismissLocator()
        {
            locatorDismissed = true;
            DisableLocator();
        }

        public void UndismissLocator()
        {
            locatorDismissed = false;
            EnableLocator();
        }
    }
}