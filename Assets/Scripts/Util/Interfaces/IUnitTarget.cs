namespace Util.Interfaces
{
    public interface IUnitTarget : ITransformProvider
    {
        public bool CanAggroUnit { get; }

        public delegate void UnitTargetEvent();
        public event UnitTargetEvent OnUnitDetach;
    }
}