using System;
using Gameplay.Bosses.Centipede;
using Gameplay.Mutations.Passive;
using Pooling;
using UnityEngine;
using Util;

namespace Gameplay.Effects.LilHorror
{
    public class LilHorrorPart : Poolable
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;
        
        private LilHorrorPart parent;
        
        private static readonly int BodyHash = Animator.StringToHash("CentipedeBody");
        private static readonly int HeadHash = Animator.StringToHash("CentipedeHead");
        private static readonly int TailHash = Animator.StringToHash("CentipedeTail");


        public override bool OnTakenFromPool(object data)
        {
            if (data is not LilHorrorPartArguments args) return false;
            parent = args.parentPart;

            if (parent is null)
                animator.Play(HeadHash);
            else
            {
                if (parent.parent is not null) 
                    parent.animator.Play(BodyHash);

                animator.Play(TailHash);
            }
            
            return base.OnTakenFromPool(data);
        }

        private void FixedUpdate()
        {
            if (parent is not null) 
                SelfMove(parent.transform.position, 180f);
        }

        /**
         * Local part movement
         */
        private void SelfMove(Vector2 target, float rotationSpeed)
        {
            Vector2 pos = transform.position;
            float distance = (target - pos).sqrMagnitude;
            rb.RotateTowardsPosition(target, rotationSpeed);
            transform.position = Vector2.MoveTowards(pos, target, distance * (1 - 0.025f));
        }
        
        /**
         * Outer part movement (head)
         */
        public float Move(Vector2 target, float speedModifier)
        {
            rb.RotateTowardsPosition(target, LegTremor.RotationSpeed * Time.fixedDeltaTime);
            rb.linearVelocity = transform.up.normalized * 
                                (LegTremor.MoveSpeed * Mathf.Clamp01(speedModifier));
            return (target - (Vector2) transform.position).sqrMagnitude;
        }
        
        public override void OnPool()
        {
            if (parent is not null)
                parent.animator.Play(TailHash);
            base.OnPool();
        }
    }
}