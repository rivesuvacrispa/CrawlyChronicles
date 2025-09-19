using System.Collections;
using UnityEngine;

namespace Gameplay.Bosses.Centipede
{
    public class CentipedeBoss : Boss
    {
        public static CentipedeBoss Instance { get; private set; }
        
        [SerializeField] private GameObject head;
        [SerializeField] private float despawnTime;

        public delegate void CentipedeBossEvent();
        public event CentipedeBossEvent OnFlee;
        
        private int fragmentsAmount;
        
        public static float FollowRotationSpeed { get; private set; }
        public static float FollowMovespeed { get; private set; }
        public static float ContactDamage { get; private set; }
        public static float AttackDamage { get; private set; }
        


        private void Awake()
        {
            Instance = this;
            FollowRotationSpeed = CentipedeDefinitions.FollowRotationSpeed;
            FollowMovespeed = CentipedeDefinitions.FollowSpeed;
            ContactDamage = CentipedeDefinitions.ContactDamage;
            AttackDamage = CentipedeDefinitions.AttackDamage;
        }

        protected override bool InvokeDestructionEvent()
        {
            if (base.InvokeDestructionEvent())
            {
                Instance = null;
                return true;
            }
            return false;
        }
        
        protected override void Enrage()
        {
            FollowRotationSpeed *= 1.5f;
            FollowMovespeed *= 1.5f;
            ContactDamage *= 1.5f;
            AttackDamage *= 1.5f;
        }

        protected override void Start()
        {
            Bossbar.Instance.SetMaxHealth(CentipedeDefinitions.FragmentHealth);
            base.Start();

            fragmentsAmount = CentipedeDefinitions.BodyLength + 1;
        }

        public override void SetLocation(Vector3 location)
        {
            transform.position = Vector3.zero;
            head.transform.localPosition = location;
        }
        
        public override void Flee()
        {
            OnFlee?.Invoke();
            base.Flee();
            StartCoroutine(DespawnRoutine());
        }

        private IEnumerator DespawnRoutine()
        {
            yield return new WaitForSeconds(despawnTime);
            Destroy(gameObject);
        }

        public void OnFragmentDeath()
        {
            fragmentsAmount--;
            if(fragmentsAmount == 0) Die();
        }
    }
}