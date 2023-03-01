using System.Collections;
using Gameplay.Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Gameplay.Bosses.Centipede
{
    public class CentipedeHead : MonoBehaviour, IEnemyAttack
    {
        [SerializeField] private bool first;
        
        private CentipedeFragment fragment;
        private float speed;

        private static readonly int HeadAnimHash = Animator.StringToHash("CentipedeHead");
        
        
        private void Awake()
        {
            fragment = GetComponent<CentipedeFragment>();
            fragment.PlayAnimation(HeadAnimHash);
            fragment.SetHead();
            fragment.UpdateColor(1);
            speed = CentipedeDefinitions.FollowRotationSpeed;

            if (first)
            {
                fragment.MaxHealth = CentipedeDefinitions.FragmentHealth;
                Bossbar.Instance.SetName("Giant Centipede");
                Bossbar.Instance.SetMaxHealth(fragment.MaxHealth);
                Bossbar.Instance.SetActive(true);
                fragment.CreateFragment(CentipedeDefinitions.BodyLength, CentipedeDefinitions.BodyLength);
            }
        }
        
        private void OnEnable()
        {
            StartCoroutine(AttackRoutine());
            StartCoroutine(ChangeSpeedRoutine());
        }

        private IEnumerator ChangeSpeedRoutine()
        {
            while (enabled)
            {
                SetSpeed(Random.Range(0.6f, 1));
                yield return new WaitForSeconds(Random.Range(3f, 6f));
            }
        }
        
        private IEnumerator DistanceRoutine()
        {
            Vector2 target = Random.insideUnitCircle.normalized * Random.Range(8f, 12f) + Player.Movement.Position;
            float distance = float.MaxValue;

            while (distance > 3f && enabled)
            {
                fragment.Move(target, speed, CentipedeDefinitions.FollowRotationSpeed);
                distance = ((Vector2) transform.position - target).sqrMagnitude;
                yield return new WaitForFixedUpdate();
            }

            if (enabled) StartCoroutine(AttackRoutine());
        }
        
        private IEnumerator AttackRoutine()
        {
            float distance = float.MaxValue;
            Vector2 target = Player.Movement.Position;
            
            while (distance > 1.25f && enabled)
            {
                fragment.Move(target, speed, CentipedeDefinitions.FollowRotationSpeed);
                distance = ((Vector2) transform.position - target).sqrMagnitude;
                yield return new WaitForFixedUpdate();
                
                target = Player.Movement.Position;
            }

            yield return new WaitForSeconds(1f);
            if(enabled) StartCoroutine(DistanceRoutine());
        }

        private void SetSpeed(float spd) => speed = CentipedeDefinitions.FollowSpeed * spd;
        
        
                        
        // IEnemyAttack
        public Vector3 AttackPosition => transform.position;
        public float AttackDamage => CentipedeDefinitions.AttackDamage;
        public float AttackPower => CentipedeDefinitions.Knockback;
    }
}