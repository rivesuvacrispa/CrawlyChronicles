namespace Util.Interfaces
{
    public interface IDestructionEventProvider
    {
        public delegate void DestructionProviderEvent(IDestructionEventProvider provider);
        public event DestructionProviderEvent OnProviderDestroy;
    }
}