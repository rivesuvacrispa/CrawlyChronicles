using Gameplay.Genes;
using Gameplay.Player.Movement;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Scriptable/Character")]
    public class Character : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private string animatorName;
        [SerializeField] private TrioGene slots;
        [SerializeField] private PlayerMovementProvider movementProvider;

        public string Name => name;
        public TrioGene MutationSlots => slots;
        public PlayerMovementProvider MovementProvider => movementProvider;
        
        
        
        public int WalkAnimHash { get; private set; }
        public int IdleAnimHash { get; private set; }
        public int DeadAnimHash { get; private set; }
        
        private void Awake() => Init();

        private void Init()
        {
            WalkAnimHash = Animator.StringToHash(animatorName + "Walk");
            IdleAnimHash = Animator.StringToHash(animatorName + "Idle");
            DeadAnimHash = Animator.StringToHash(animatorName + "Dead");
        }
        
        private void OnValidate() => Init();
    }
}