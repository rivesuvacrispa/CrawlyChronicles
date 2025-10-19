using Gameplay.Mutations.EntityEffects;

namespace Util.Interfaces
{
    public interface IEffectAffectable : IImpactable
    {
        public EffectController EffectController { get; }
        public bool CanApplyEffect { get; }

        public void AddEffect<T>(EntityEffectData data) where T : EntityEffect
        {
            if (CanApplyEffect) EffectController.AddEffect<T>(data);
        }
    }
}