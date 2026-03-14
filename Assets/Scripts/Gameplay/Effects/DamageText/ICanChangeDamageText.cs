namespace Gameplay.Effects.DamageText
{
    public interface ICanChangeDamageText
    {
        public bool ShouldChangeDamageText();
        public DamageTextProperties GetDamageTextProperties();
    }
}