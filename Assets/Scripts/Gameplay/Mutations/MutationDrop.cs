using Gameplay.Breeding;
using Gameplay.Player;
using Scriptable;
using UnityEngine;
using Util;

namespace Gameplay.Mutations
{
    public class MutationDrop : PickupableObject
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private BasicMutation mutation;

        public void SetData(BasicMutation basicMutation)
        {
            mutation = basicMutation;
            spriteRenderer.sprite = mutation.Sprite;
            spriteRenderer.color = mutation.SpriteColor;
        }

        protected override void OnPickup()
        {
            var mutations = AbilityController.GetMutationData();
            mutations.Add(mutation, 0);
            Egg egg = new Egg(BreedingManager.Instance.TrioGene, mutations);
            AbilityController.UpdateAbilities(egg);
        }
    }
}