using Gameplay.Abilities.EntityEffects;

namespace Scripts.Util.Interfaces
{
    public interface IImpactEffectAffectable : IImpactable
    {
        public void AddEffect<T>(EntityEffectData data) where T : EntityEffect;
    }
}