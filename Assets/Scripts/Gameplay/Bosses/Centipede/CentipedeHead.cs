using System.Collections;
using Gameplay.Map;
using UnityEngine;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Bosses.Centipede
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
            SetSpeed(1);
            CentipedeBoss.Instance.OnFlee += OnFlee;

            if (first)
            {
                fragment.MaxHealth = CentipedeDefinitions.FragmentHealth;
                fragment.CreateFragment(CentipedeDefinitions.BodyLength, CentipedeDefinitions.BodyLength);
            }
        }
        
        private void OnEnable()
        {
            StartCoroutine(AttackRoutine());
            StartCoroutine(ChangeSpeedRoutine());
        }

        private void OnFlee()
        {
            StopAllCoroutines();
            SetSpeed(2f);
            StartCoroutine(FleeRoutine());
        }

        private IEnumerator ChangeSpeedRoutine()
        {
            while (enabled)
            {
                SetSpeed(Random.Range(0.6f, 1));
                yield return new WaitForSeconds(Random.Range(3f, 6f));
            }
        }

        private IEnumerator FleeRoutine()
        {
            CentipedeBoss.Instance.OnFlee -= OnFlee;
            Vector2 pos = MapManager.GetRandomPointAroundMap(100);
            while (enabled)
            {
                fragment.Move(pos, speed, CentipedeBoss.FollowRotationSpeed);
                yield return new WaitForFixedUpdate();
            }
        }
        
        private IEnumerator DistanceRoutine()
        {
            Vector2 target = Random.insideUnitCircle.normalized * Random.Range(6f, 8f) + Player.PlayerPhysicsBody.Position;
            float distance = float.MaxValue;

            while (distance > 3f && enabled)
            {
                fragment.Move(target, speed, CentipedeBoss.FollowRotationSpeed);
                distance = ((Vector2) transform.position - target).sqrMagnitude;
                yield return new WaitForFixedUpdate();
            }

            if (enabled) StartCoroutine(AttackRoutine());
        }
        
        private IEnumerator AttackRoutine()
        {
            float distance = float.MaxValue;
            Vector2 target = Player.PlayerPhysicsBody.Position;
            
            while (distance > 1.1f && enabled)
            {
                fragment.Move(target, speed, CentipedeBoss.FollowRotationSpeed);
                distance = ((Vector2) transform.position - target).sqrMagnitude;
                yield return new WaitForFixedUpdate();
                
                target = Player.PlayerPhysicsBody.Position;
            }

            yield return new WaitForSeconds(1f);
            if(enabled) StartCoroutine(DistanceRoutine());
        }

        private void SetSpeed(float spd) => speed = CentipedeBoss.FollowMovespeed * spd;

        private void OnDestroy()
        {
            if(CentipedeBoss.Instance is not null)
                CentipedeBoss.Instance.OnFlee -= OnFlee;
        }


        // IEnemyAttack
        public Vector3 AttackPosition => transform.position;
        public float AttackDamage => CentipedeBoss.AttackDamage;
        public float AttackPower => CentipedeDefinitions.Knockback;
    }
}